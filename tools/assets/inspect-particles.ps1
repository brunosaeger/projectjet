<#
.SYNOPSIS
  Resume os ParticleSystems de um prefab do jogo (duração, vida, tamanho, velocidade, cor) para saber qual peça é qual.
.EXAMPLE
  ./tools/assets/inspect-particles.ps1 -BundleFilter "ror2-base-commando_static*" -Prefab CommandoDashJets
#>
param(
    [Parameter(Mandatory)][string]$BundleFilter,
    [Parameter(Mandatory)][string]$Prefab,
    [string]$GameDir = "D:\SteamLibrary\steamapps\common\Risk of Rain 2"
)

$ErrorActionPreference = "Stop"
$bundles = Join-Path $GameDir "Risk of Rain 2_Data\StreamingAssets\aa\StandaloneWindows64"
$script = Join-Path $PSScriptRoot "inspect_particles.py"

foreach ($bundle in Get-ChildItem $bundles -Filter "$BundleFilter.bundle") {
    Write-Host "##### $($bundle.Name)"
    uv run --no-project --with UnityPy python -I $script $bundle.FullName $Prefab
}
