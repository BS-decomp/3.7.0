param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectPath
)

$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $ProjectPath).Path

if (Test-Path -LiteralPath (Join-Path $root 'Temp/UnityLockfile')) {
    throw 'Close Unity first.'
}
if (!(Test-Path -LiteralPath (Join-Path $root 'Assets/Scripts/Assembly-CSharp/LevelManager.cs'))) {
    throw 'Not the expected Block Strike Unity project.'
}

$toolSource = Join-Path $PSScriptRoot 'unity-editor/BlockStrikeGeometryRecovery.cs'
$manifestSource = Join-Path $PSScriptRoot 'unity-editor/MapGeometry'
if (!(Test-Path -LiteralPath $toolSource)) { throw 'Missing editor recovery script.' }
if (!(Test-Path -LiteralPath $manifestSource)) { throw 'Missing geometry manifests.' }
$manifests = @(Get-ChildItem -LiteralPath $manifestSource -Filter '*.json' -File)
if ($manifests.Count -ne 56) { throw "Expected 56 geometry manifests, found $($manifests.Count)." }

$destination = Join-Path $root 'Assets/Editor/BlockStrikeRecovery'
$manifestDestination = Join-Path $destination 'MapGeometry'
$backup = Join-Path $root ('RecoveryBackups/GeometryInstaller-' + [guid]::NewGuid().ToString('N'))

New-Item -ItemType Directory -Force -Path $destination | Out-Null
New-Item -ItemType Directory -Force -Path $manifestDestination | Out-Null

function Backup-File([string]$Path) {
    if (Test-Path -LiteralPath $Path) {
        New-Item -ItemType Directory -Force -Path $backup | Out-Null
        $relative = $Path.Substring($root.Length).TrimStart('\', '/')
        $target = Join-Path $backup $relative
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target) | Out-Null
        Copy-Item -LiteralPath $Path -Destination $target -Force
    }
}

# The pilot manifest is obsolete and is ignored by the universal tool.
$legacyPilot = Join-Path $destination 'BustGeometry.json'
if (Test-Path -LiteralPath $legacyPilot) {
    Backup-File $legacyPilot
    Remove-Item -LiteralPath $legacyPilot -Force
}

Backup-File (Join-Path $destination 'BlockStrikeGeometryRecovery.cs')
Copy-Item -LiteralPath $toolSource -Destination (Join-Path $destination 'BlockStrikeGeometryRecovery.cs') -Force

foreach ($manifest in $manifests) {
    $target = Join-Path $manifestDestination $manifest.Name
    Backup-File $target
    Copy-Item -LiteralPath $manifest.FullName -Destination $target -Force
}

Write-Host 'Installed the universal editor recovery tool: 1 script and 56 manifests.'
Write-Host 'No scene, runtime script, camera, event, collider or material files were changed.'
Write-Host 'Open Unity, then: Tools > Block Strike Recovery > Repair ALL scene geometry.'
