<#
.SYNOPSIS
  Inspeciona prefabs dentro dos asset bundles do RoR2 (somente leitura): componentes, campos e filhos.
.DESCRIPTION
  Útil quando o código decompilado não basta (valores ficam nos assets: efeitos, sons, tremor, raios...).
  Usa UnityPy num ambiente temporário do uv (nada é instalado no sistema).
  Os nomes das classes dos scripts ficam em outro bundle; identifique pelos nomes dos campos.
.EXAMPLE
  ./tools/assets/inspect-bundle.ps1 -BundleFilter "ror2-base-captain*" -Prefabs CaptainAirstrikeProjectile1,CaptainAirstrikeImpact1
#>
param(
    [Parameter(Mandatory)][string]$BundleFilter,
    [Parameter(Mandatory)][string[]]$Prefabs,
    [string]$GameDir = "D:\SteamLibrary\steamapps\common\Risk of Rain 2"
)

$ErrorActionPreference = "Stop"
$bundles = Join-Path $GameDir "Risk of Rain 2_Data\StreamingAssets\aa\StandaloneWindows64"
$script = Join-Path $PSScriptRoot "inspect_bundle.py"

foreach ($bundle in Get-ChildItem $bundles -Filter "$BundleFilter.bundle") {
    Write-Host "##### $($bundle.Name)"
    uv run --no-project --with UnityPy python -I $script $bundle.FullName ($Prefabs -join ",")
}
