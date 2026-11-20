<#
.SYNOPSIS
  Instala no perfil de teste os mods de tools/profile-mods.txt (última versão, com dependências).
.DESCRIPTION
  Segue as regras de instalação do r2modman para BepInEx: as pastas plugins/patchers/monomod/config do pacote
  vão para BepInEx/<pasta>/<Namespace-Nome>/; arquivos soltos vão para BepInEx/plugins/<Namespace-Nome>/.
  O BepInExPack é ignorado (quem instala é o setup-profile.ps1).
#>
param(
    [string]$ProfileName = "JetDev",
    [string]$ProfilesRoot = "$env:APPDATA\r2modmanPlus-local\RiskOfRain2\profiles",
    [string]$ModList = (Join-Path $PSScriptRoot "profile-mods.txt")
)

$ErrorActionPreference = "Stop"
$bepinex = Join-Path $ProfilesRoot "$ProfileName\BepInEx"
if (-not (Test-Path $bepinex)) { throw "Perfil sem BepInEx. Rode tools/setup-profile.ps1 primeiro." }

$installed = @{}
$temp = Join-Path ([IO.Path]::GetTempPath()) "jetproject-mods"
New-Item -ItemType Directory -Force $temp | Out-Null

function Install-Package([string]$fullName) {
    $ns, $name = $fullName -split '-', 2
    if ($installed.ContainsKey($fullName) -or $fullName -eq "bbepis-BepInExPack") { return }
    $installed[$fullName] = $true

    $pkg = Invoke-RestMethod "https://thunderstore.io/api/experimental/package/$ns/$name/"
    foreach ($dep in $pkg.latest.dependencies) {
        Install-Package (($dep -split '-')[0..1] -join '-')
    }

    Write-Host "Instalando $fullName $($pkg.latest.version_number)"
    $zip = Join-Path $temp "$fullName.zip"
    $extract = Join-Path $temp $fullName
    Remove-Item $extract -Recurse -Force -ErrorAction SilentlyContinue
    Invoke-WebRequest $pkg.latest.download_url -OutFile $zip
    Expand-Archive $zip -DestinationPath $extract -Force

    Copy-PackageContent $extract $fullName
}

function Copy-PackageContent([string]$root, [string]$fullName) {
    $routed = @("plugins", "patchers", "monomod", "config")
    $metadata = @("manifest.json", "README.md", "CHANGELOG.md", "icon.png", "LICENSE", "LICENSE.md")
    foreach ($item in Get-ChildItem $root) {
        if ($item.PSIsContainer -and $item.Name -eq "BepInEx") {
            # Pacotes que espelham a estrutura do BepInEx: aplica as mesmas regras dentro dela
            Copy-PackageContent $item.FullName $fullName
        }
        elseif ($item.PSIsContainer -and $routed -contains $item.Name.ToLower()) {
            $dest = if ($item.Name -eq "config") { Join-Path $bepinex "config" } else { Join-Path $bepinex "$($item.Name.ToLower())\$fullName" }
            New-Item -ItemType Directory -Force $dest | Out-Null
            Copy-Item (Join-Path $item.FullName "*") $dest -Recurse -Force
        }
        elseif ($metadata -notcontains $item.Name) {
            $dest = Join-Path $bepinex "plugins\$fullName"
            New-Item -ItemType Directory -Force $dest | Out-Null
            Copy-Item $item.FullName $dest -Recurse -Force
        }
    }
}

Get-Content $ModList | ForEach-Object { $_.Trim() } | Where-Object { $_ -and -not $_.StartsWith("#") } | ForEach-Object { Install-Package $_ }
Remove-Item $temp -Recurse -Force
Write-Host "Mods instalados no perfil '$ProfileName'."
