<#
.SYNOPSIS
  Abre o Risk of Rain 2 modado com o perfil de teste, igual ao botão "Start modded" do r2modman.
.DESCRIPTION
  Usa os argumentos do Doorstop 4 apontando para o BepInEx do perfil.
  Pré-requisito: abrir o jogo modado UMA vez pelo r2modman, que instala o winhttp.dll na pasta do jogo.
.EXAMPLE
  ./tools/launch.ps1 -Build
#>
param(
    [string]$ProfileName = "JetDev",
    [string]$GameDir = "D:\SteamLibrary\steamapps\common\Risk of Rain 2",
    [switch]$Build
)

$ErrorActionPreference = "Stop"
$root = Split-Path $PSScriptRoot -Parent
$profileDir = "$env:APPDATA\r2modmanPlus-local\RiskOfRain2\profiles\$ProfileName"
$preloader = Join-Path $profileDir "BepInEx\core\BepInEx.Preloader.dll"

if ($Build) {
    dotnet build (Join-Path $root "JetProject.sln") -c Debug
    if ($LASTEXITCODE -ne 0) { throw "Build falhou." }
}

if (-not (Test-Path $preloader)) { throw "Perfil sem BepInEx. Rode tools/setup-profile.ps1." }
if (-not (Test-Path (Join-Path $GameDir "winhttp.dll"))) {
    throw "winhttp.dll ausente na pasta do jogo. Abra o perfil '$ProfileName' uma vez pelo r2modman (Start modded)."
}

$steam = (Get-ItemProperty "HKCU:\Software\Valve\Steam").SteamExe
Start-Process $steam -ArgumentList @(
    "-applaunch", "632360",
    "--doorstop-enabled", "true",
    "--doorstop-target-assembly", "`"$preloader`""
)
Write-Host "Jogo iniciado com o perfil '$ProfileName'. Log: $profileDir\BepInEx\LogOutput.log"
