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
$manifestFolder = Join-Path $root 'Assets/Editor/BlockStrikeRecovery/MapGeometry'
if (!(Test-Path -LiteralPath $manifestFolder)) {
    throw 'Install geometry recovery first; missing Assets/Editor/BlockStrikeRecovery/MapGeometry.'
}
$manifests = @(Get-ChildItem -LiteralPath $manifestFolder -Filter '*.json' -File)
if ($manifests.Count -ne 56) {
    throw "Expected 56 geometry manifests, found $($manifests.Count)."
}

$sourceRoot = Join-Path $PSScriptRoot 'unity-editor'
$backupRoot = Join-Path $root ('RecoveryBackups/LightmapInstaller-' + [guid]::NewGuid().ToString('N'))
$runtimeSource = Join-Path $sourceRoot 'LegacyLightmapBinder.cs'
$runtimeDestination = Join-Path $root 'Assets/Scripts/Assembly-CSharp/LegacyLightmapBinder.cs'
$editorSource = Join-Path $sourceRoot 'BlockStrikeLightmapRecovery.cs'
$editorDestination = Join-Path $root 'Assets/Editor/BlockStrikeRecovery/BlockStrikeLightmapRecovery.cs'
$items = @(
    @{ Source = $runtimeSource; Destination = $runtimeDestination }
    @{ Source = $editorSource; Destination = $editorDestination }
)

function Backup-File([string]$Path) {
    if (Test-Path -LiteralPath $Path) {
        New-Item -ItemType Directory -Force -Path $backupRoot | Out-Null
        $relative = $Path.Substring($root.Length).TrimStart('\', '/')
        $target = Join-Path $backupRoot $relative
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target) | Out-Null
        Copy-Item -LiteralPath $Path -Destination $target -Force
    }
}

foreach ($item in $items) {
    if (!(Test-Path -LiteralPath $item.Source)) { throw "Missing source file: $($item.Source)" }
    $sourceMeta = $item.Source + '.meta'
    $destinationMeta = $item.Destination + '.meta'
    if (!(Test-Path -LiteralPath $sourceMeta)) { throw "Missing Unity .meta source: $sourceMeta" }

    $destinationFolder = Split-Path -Parent $item.Destination
    New-Item -ItemType Directory -Force -Path $destinationFolder | Out-Null
    $sameSource = $false
    if (Test-Path -LiteralPath $item.Destination) {
        $sourceText = [System.IO.File]::ReadAllText($item.Source)
        $targetText = [System.IO.File]::ReadAllText($item.Destination)
        $sameSource = $sourceText -ceq $targetText
    }

    if (!$sameSource) {
        Backup-File $item.Destination
        # Preserve the project's existing GUID on upgrades: scene components may reference it.
        if (!(Test-Path -LiteralPath $destinationMeta)) { Backup-File $destinationMeta }
        Copy-Item -LiteralPath $item.Source -Destination $item.Destination -Force
        if (!(Test-Path -LiteralPath $destinationMeta)) {
            Copy-Item -LiteralPath $sourceMeta -Destination $destinationMeta -Force
        }
    } elseif (!(Test-Path -LiteralPath $destinationMeta)) {
        Copy-Item -LiteralPath $sourceMeta -Destination $destinationMeta -Force
    }
}

Write-Host 'Installed the reversible legacy-lightmap editor tool and runtime binder.'
Write-Host 'No scenes or lightmap texture settings were changed by this installer.'
Write-Host 'After geometry repair, run: Tools > Block Strike Recovery > Validate ALL legacy lightmaps.'
Write-Host 'Then run: Tools > Block Strike Recovery > Bind ALL legacy lightmaps.'
if (Test-Path -LiteralPath $backupRoot) { Write-Host "Replaced-file backups: $backupRoot" }
