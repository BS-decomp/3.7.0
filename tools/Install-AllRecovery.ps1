# ONE installer to rule them all: applies every Block Strike 608 recovery step
# to a fresh AssetRipper project copy (any path), in a safe order.
# The repo's client/ already has its shader, scene-name and geometry data. The lightmap binder is opt-in.
param(
    [Parameter(Mandatory=$true)][string]$ProjectPath,
    [switch]$RestoreBuildSettings
)
$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$root = (Resolve-Path -LiteralPath $ProjectPath).Path
if (Test-Path -LiteralPath (Join-Path $root 'Temp/UnityLockfile')) {
    throw 'Close Unity before running this tool.'
}
if (!(Test-Path -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/LevelManager.cs'))) {
    throw 'Not the expected Block Strike Unity project.'
}

Write-Host '[1/6] Unity 5.6 C# compile fixes (7 guarded source patches)...'
& (Join-Path $here 'Apply-Unity56Fixes.ps1') -ProjectPath $root

Write-Host '[2/6] Readable scene names (56 renames + LevelManager + Build Settings)...'
if ($RestoreBuildSettings) {
    & (Join-Path $here 'Restore-SceneNames.ps1') -ProjectPath $root -RestoreBuildSettings
} else {
    & (Join-Path $here 'Restore-SceneNames.ps1') -ProjectPath $root
}

Write-Host '[3/6] Lightmap folder names (GUID-preserving)...'
& (Join-Path $here 'Normalize-LightmapPaths.ps1') -ProjectPath $root

Write-Host '[4/6] 35 rebuilt shaders over DummyShaderTextExporter placeholders...'
& (Join-Path $here 'Install-ShaderRecovery.ps1') -ProjectPath $root

Write-Host '[5/6] Universal geometry recovery editor tool (56 manifests)...'
& (Join-Path $here 'Install-GeometryRecovery.ps1') -ProjectPath $root

Write-Host '[6/6] Reversible legacy lightmap binder and editor commands...'
& (Join-Path $here 'Install-LightmapRecovery.ps1') -ProjectPath $root

Write-Host ''
Write-Host 'ALL DONE. Now open the project in Unity 5.6.7f1, wait for compile/import,'
Write-Host 'then run: Tools > Block Strike Recovery > Repair ALL scene geometry.'
Write-Host 'After that, run Validate ALL legacy lightmaps, then Bind ALL legacy lightmaps.'
