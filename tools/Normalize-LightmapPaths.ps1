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

$mapFile = Join-Path $PSScriptRoot 'scene-names.json'
if (!(Test-Path -LiteralPath $mapFile)) { throw 'Missing scene-names.json next to this script.' }
$scenes = Get-Content -LiteralPath $mapFile -Raw | ConvertFrom-Json

$movedFolders = 0
$movedTextures = 0
$already = 0
$skipped = 0

foreach ($scene in $scenes) {
    $parent = Split-Path -Parent $scene.new
    $stem = [System.IO.Path]::GetFileNameWithoutExtension($scene.old)
    $encDir = Join-Path $root (Join-Path $parent $stem)
    $destDir = Join-Path $root (Join-Path $parent $scene.name)

    if (!(Test-Path -LiteralPath $encDir)) {
        if (Test-Path -LiteralPath $destDir) { $already++ }
        else { $skipped++ }
        continue
    }
    if (Test-Path -LiteralPath $destDir) {
        throw "Both exist: $encDir and $destDir - resolve manually before continuing."
    }

    $encMeta = $encDir + '.meta'
    $destMeta = $destDir + '.meta'

    Move-Item -LiteralPath $encDir -Destination $destDir
    if (Test-Path -LiteralPath $encMeta) {
        Move-Item -LiteralPath $encMeta -Destination $destMeta
    }

    $textures = @(Get-ChildItem -LiteralPath $destDir -Filter '*.png' -File -ErrorAction SilentlyContinue)
    foreach ($tex in $textures) {
        if (!(Test-Path -LiteralPath ($tex.FullName + '.meta'))) {
            throw "Lost the .meta (GUID) for $($tex.FullName)"
        }
        $movedTextures++
    }
    $movedFolders++
    Write-Host ("{0}  ->  {1}" -f ($parent + '/' + $stem), ($parent + '/' + $scene.name))
}

Write-Host ''
Write-Host "Lightmap folders normalized: $movedFolders (textures moved: $movedTextures)."
Write-Host "Already readable: $already. Scenes without lightmap data: $skipped."
Write-Host 'All .meta files moved together with the textures, so every GUID and every'
Write-Host 'scene reference still resolves. Open Unity and wait for the reimport.'
