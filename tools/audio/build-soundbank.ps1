<#
.SYNOPSIS
  Gera o soundbank Wwise do mod a partir de wwise/sounds.json e o copia para src/JetProject/Resources/SoundBanks.
.DESCRIPTION
  Pipeline totalmente automatizado via WAAPI (HTTP), sem abrir a interface do Wwise:
    1. Converte cada fonte (art/audio/source/*) para WAV com ffmpeg.
    2. Sobe o WwiseConsole em modo waapi-server com o projeto wwise/JetProject.
    3. Garante a estrutura: bus Character (espelho do jogo), attenuation, actor-mixer 3D.
    4. Importa os sons (substituindo) e cria um evento Play_<Nome> para cada um; remove o que saiu do manifesto.
    5. Gera o bank para Windows, valida a versão (150 = mesma do jogo) e copia para Resources/SoundBanks.

  Para adicionar um som: coloque a fonte em art/audio/source/ e uma entrada em wwise/sounds.json.
#>
param(
    [string]$WwiseRoot = "D:\Audiokinetic\Wwise2023.1.4.8496",
    [int]$HttpPort = 8090
)

$ErrorActionPreference = "Stop"
$root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$projectFile = Join-Path $root "wwise\JetProject\JetProject.wproj"
$manifest = Get-Content (Join-Path $root "wwise\sounds.json") -Raw | ConvertFrom-Json
$console = Join-Path $WwiseRoot "Authoring\x64\Release\bin\WwiseConsole.exe"
$expectedBankVersion = 150

$mixerPath = "\Actor-Mixer Hierarchy\Default Work Unit\JetProject"
$eventsPath = "\Events\Default Work Unit"
$bankName = $manifest.soundbank

foreach ($tool in @($console)) { if (-not (Test-Path $tool)) { throw "Não encontrado: $tool" } }
if (-not (Get-Command ffmpeg -ErrorAction SilentlyContinue)) { throw "ffmpeg não encontrado (winget install Gyan.FFmpeg)." }

function Invoke-Waapi([string]$uri, [hashtable]$arguments = @{}, [hashtable]$options = @{}) {
    $body = @{ uri = $uri; args = $arguments; options = $options } | ConvertTo-Json -Depth 20 -Compress
    try {
        return Invoke-RestMethod -Uri "http://127.0.0.1:$HttpPort/waapi" -Method Post -ContentType "application/json" -Body $body
    }
    catch {
        throw "WAAPI $uri falhou: $($_.ErrorDetails.Message)"
    }
}

function Get-Children([string]$path) {
    $r = Invoke-Waapi "ak.wwise.core.object.get" @{ from = @{ path = @($path) }; transform = @(@{ select = @("children") }) } @{ return = @("name", "type", "path") }
    return @($r.return)
}

function Ensure-Object([string]$parent, [string]$type, [string]$name) {
    Invoke-Waapi "ak.wwise.core.object.create" @{ parent = $parent; type = $type; name = $name; onNameConflict = "merge" } | Out-Null
    return "$parent\$name"
}

function Get-PeakDb([string]$file, [string]$filter = "anull") {
    $out = ffmpeg -hide_banner -i $file -af "$filter,volumedetect" -f null NUL 2>&1 | Out-String
    $m = [regex]::Match($out, 'max_volume:\s*(-?[\d\.]+) dB')
    if (-not $m.Success) { throw "Não foi possível medir o volume de $file" }
    return [double]::Parse($m.Groups[1].Value, [Globalization.CultureInfo]::InvariantCulture)
}

# 1. Conversão das fontes: mono 48 kHz + normalização de pico (todos os sons entram no mesmo nível;
#    o volumeDb do manifesto fica só para mixagem relativa entre eles).
$normalizePeakDb = -1.0
$work = Join-Path ([IO.Path]::GetTempPath()) "jetproject-wwise"
Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $work | Out-Null
foreach ($sound in $manifest.sounds) {
    $src = Join-Path $root $sound.source
    if (-not (Test-Path $src)) { throw "Fonte não encontrada: $($sound.source)" }

    $mono = "aformat=channel_layouts=mono"
    $gain = $normalizePeakDb - (Get-PeakDb $src $mono)
    $gainText = $gain.ToString("0.00", [Globalization.CultureInfo]::InvariantCulture)
    ffmpeg -y -v error -i $src -af "$mono,volume=${gainText}dB" -ar 48000 -c:a pcm_s16le (Join-Path $work "$($sound.name).wav")
    if ($LASTEXITCODE -ne 0) { throw "ffmpeg falhou em $($sound.source)" }
    Write-Host ("{0}: normalizado {1:+0.0;-0.0} dB (pico em {2} dBFS)" -f $sound.name, $gain, $normalizePeakDb)
}

# 2. Servidor WAAPI
$server = Start-Process $console -ArgumentList @("waapi-server", "`"$projectFile`"", "--http-port", $HttpPort, "--wamp-port", ($HttpPort + 5)) -PassThru -WindowStyle Hidden
try {
    $ready = $false
    for ($i = 0; $i -lt 120 -and -not $ready; $i++) {
        try { Invoke-Waapi "ak.wwise.core.getInfo" | Out-Null; $ready = $true } catch { Start-Sleep -Milliseconds 500 }
    }
    if (-not $ready) { throw "WAAPI não respondeu." }

    # 3. Estrutura
    # Bus com o MESMO caminho/nome do jogo: o ID (hash do nome) coincide com o bus Character do Init.bnk do RoR2,
    # então os sons herdam volume Master/SFX e a mixagem HDR do jogo. O bus em si não vai para o nosso bank.
    $bus = "\Master-Mixer Hierarchy\Default Work Unit\Master Audio Bus"
    $bus = Ensure-Object $bus "Bus" "SFX_BUS"
    $bus = Ensure-Object $bus "Bus" "HDR"
    $bus = Ensure-Object $bus "Bus" "Character"

    $attenuation = Ensure-Object "\Attenuations\Default Work Unit" "Attenuation" "JetProject_Default"
    Invoke-Waapi "ak.wwise.core.object.setProperty" @{ object = $attenuation; property = "RadiusMax"; value = 100 } | Out-Null

    Ensure-Object "\Actor-Mixer Hierarchy\Default Work Unit" "ActorMixer" "JetProject" | Out-Null
    foreach ($p in @(@{ n = "OverrideOutput"; v = $true }, @{ n = "OverridePositioning"; v = $true }, @{ n = "3DSpatialization"; v = 1 }, @{ n = "EnableAttenuation"; v = $true })) {
        Invoke-Waapi "ak.wwise.core.object.setProperty" @{ object = $mixerPath; property = $p.n; value = $p.v } | Out-Null
    }
    Invoke-Waapi "ak.wwise.core.object.setReference" @{ object = $mixerPath; reference = "OutputBus"; value = $bus } | Out-Null
    Invoke-Waapi "ak.wwise.core.object.setReference" @{ object = $mixerPath; reference = "Attenuation"; value = $attenuation } | Out-Null

    # 4. Sons e eventos
    $names = @($manifest.sounds | ForEach-Object { $_.name })
    $imports = @($manifest.sounds | ForEach-Object {
        @{
            audioFile  = (Join-Path $work "$($_.name).wav")
            objectPath = "$mixerPath\<Sound SFX>$($_.name)"
            event      = "$eventsPath\Play_$($_.name)@Play"
        }
    })
    Invoke-Waapi "ak.wwise.core.audio.import" @{ importOperation = "replaceExisting"; default = @{ importLanguage = "SFX" }; imports = $imports } | Out-Null
    foreach ($sound in $manifest.sounds) {
        Invoke-Waapi "ak.wwise.core.object.setProperty" @{ object = "$mixerPath\$($sound.name)"; property = "Volume"; value = [double]$sound.volumeDb } | Out-Null
    }

    foreach ($child in Get-Children $mixerPath) {
        if ($names -notcontains $child.name) {
            Invoke-Waapi "ak.wwise.core.object.delete" @{ object = $child.path } | Out-Null
            Write-Host "Removido do projeto: $($child.name)"
        }
    }
    foreach ($evt in Get-Children $eventsPath) {
        if ($evt.type -eq "Event" -and $names -notcontains ($evt.name -replace '^Play_', '')) {
            Invoke-Waapi "ak.wwise.core.object.delete" @{ object = $evt.path } | Out-Null
            Write-Host "Evento removido: $($evt.name)"
        }
    }

    # 5. Bank
    $bankPath = Ensure-Object "\SoundBanks\Default Work Unit" "SoundBank" $bankName
    Invoke-Waapi "ak.wwise.core.soundbank.setInclusions" @{ soundbank = $bankPath; operation = "replace"; inclusions = @(@{ object = $eventsPath; filter = @("events", "structures", "media") }) } | Out-Null
    Invoke-Waapi "ak.wwise.core.soundbank.generate" @{ soundbanks = @(@{ name = $bankName }); platforms = @("Windows"); writeToDisk = $true } | Out-Null
    Invoke-Waapi "ak.wwise.core.project.save" | Out-Null
}
finally {
    if ($server -and -not $server.HasExited) { Stop-Process -Id $server.Id -Force }
    Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue
}

$generated = Join-Path $root "wwise\JetProject\GeneratedSoundBanks\Windows\$bankName.bnk"
$bytes = [IO.File]::ReadAllBytes($generated)
$version = [BitConverter]::ToUInt32($bytes, 8)
if ([Text.Encoding]::ASCII.GetString($bytes, 0, 4) -ne "BKHD" -or $version -ne $expectedBankVersion) {
    throw "Bank com versão $version (esperado $expectedBankVersion). Verifique a versão do Wwise."
}

$dest = Join-Path $root "src\JetProject\Resources\SoundBanks"
New-Item -ItemType Directory -Force $dest | Out-Null
Copy-Item $generated (Join-Path $dest "$bankName.bnk") -Force
Write-Host "Soundbank '$bankName' (v$version, $([math]::Round($bytes.Length / 1KB)) KB) -> $dest"
Write-Host "Eventos: $(($manifest.sounds | ForEach-Object { "Play_$($_.name)" }) -join ', ')"
