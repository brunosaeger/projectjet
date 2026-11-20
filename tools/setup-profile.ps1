<#
.SYNOPSIS
  Cria (ou repara) o perfil de teste do r2modman com o BepInExPack mais recente do Thunderstore.
.DESCRIPTION
  O perfil aparece no r2modman com o nome informado. Outros mods (R2API etc.) podem ser
  instalados por lá normalmente. O build do projeto copia o plugin para <perfil>\BepInEx\plugins\JetProject.
#>
param(
    [string]$ProfileName = "JetDev",
    [string]$ProfilesRoot = "$env:APPDATA\r2modmanPlus-local\RiskOfRain2\profiles",
    [switch]$Force
)

$ErrorActionPreference = "Stop"
$profileDir = Join-Path $ProfilesRoot $ProfileName

if ((Test-Path (Join-Path $profileDir "BepInEx\core")) -and -not $Force) {
    Write-Host "Perfil '$ProfileName' já possui BepInEx em $profileDir (use -Force para reinstalar)."
    return
}

$package = Invoke-RestMethod "https://thunderstore.io/api/experimental/package/bbepis/BepInExPack/"
$version = $package.latest.version_number
Write-Host "Baixando BepInExPack $version..."

$temp = Join-Path ([IO.Path]::GetTempPath()) "jetproject-bepinex-$version"
Remove-Item $temp -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force $temp | Out-Null
$zip = Join-Path $temp "pack.zip"
Invoke-WebRequest $package.latest.download_url -OutFile $zip
Expand-Archive $zip -DestinationPath (Join-Path $temp "x") -Force

# O pacote do Thunderstore traz o conteúdo do perfil dentro da pasta BepInExPack/
$packRoot = Join-Path $temp "x\BepInExPack"
New-Item -ItemType Directory -Force $profileDir | Out-Null
Copy-Item (Join-Path $packRoot "*") $profileDir -Recurse -Force

Remove-Item $temp -Recurse -Force
Write-Host "BepInEx instalado em: $profileDir"

& (Join-Path $PSScriptRoot "install-mods.ps1") -ProfileName $ProfileName -ProfilesRoot $ProfilesRoot
Write-Host "Perfil pronto: $profileDir"
