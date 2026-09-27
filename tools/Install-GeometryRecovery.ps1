param([Parameter(Mandatory=$true)][string]$ProjectPath)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $ProjectPath).Path
if (Test-Path -LiteralPath (Join-Path $root 'Temp/UnityLockfile')) { throw 'Close Unity first.' }
if (!(Test-Path -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/LevelManager.cs'))) { throw 'Not the expected project.' }
$files = @('BlockStrikeGeometryRecovery.cs', 'BustGeometry.json')
foreach ($name in $files) {
    if (!(Test-Path -LiteralPath (Join-Path $PSScriptRoot ('unity-editor/' + $name)))) { throw "Missing tool file: $name" }
}
$dest = Join-Path $root 'Assets/Editor/BlockStrikeRecovery'
$backup = Join-Path $root ('RecoveryBackups/GeometryInstaller-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $dest | Out-Null
foreach ($name in $files) {
    $target = Join-Path $dest $name
    if (Test-Path -LiteralPath $target) {
        New-Item -ItemType Directory -Force -Path $backup | Out-Null
        Copy-Item -LiteralPath $target -Destination (Join-Path $backup $name)
    }
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot ('unity-editor/' + $name)) -Destination $target -Force
}
Write-Host 'Installed editor tool only. No scenes or runtime scripts were changed.'
Write-Host 'Open Unity, then Tools > Block Strike Recovery > Repair Bust geometry.'
