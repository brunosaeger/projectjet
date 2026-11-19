<#
.SYNOPSIS
  Executa um script bpy de tools/blender/scripts em modo headless (sem abrir a interface).
.EXAMPLE
  ./tools/blender/run.ps1 inspect_scene.py -Blend art/blender/Survivor.blend
  ./tools/blender/run.ps1 export_fbx.py -Blend art/blender/Survivor.blend -ScriptArgs "--out","unity/Assets/JetProject/Models/Survivor.fbx"
#>
param(
    [Parameter(Mandatory)][string]$Script,
    [string]$Blend,
    [string[]]$ScriptArgs = @(),
    [string]$BlenderExe = "D:\blender.exe"
)

$ErrorActionPreference = "Stop"
$scriptPath = Join-Path $PSScriptRoot "scripts\$Script"
if (-not (Test-Path $scriptPath)) { throw "Script não encontrado: $scriptPath" }

$argsList = @("--background")
if ($Blend) { $argsList += (Resolve-Path $Blend).Path }
$argsList += @("--factory-startup", "--python-exit-code", "1", "--python", $scriptPath, "--") + $ScriptArgs

& $BlenderExe @argsList
exit $LASTEXITCODE
