param([Parameter(Mandatory=$true)][string]$ProjectPath)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $ProjectPath).Path
if (Test-Path -LiteralPath (Join-Path $root 'Temp/UnityLockfile')) {
    throw 'Close Unity before running this tool.'
}
# Windows PowerShell 5.1 emits the JSON array as one pipeline object.
# Assign it directly first; wrapping that pipeline in @() creates a nested array.
$parsedMap = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'scene-names.json') -Raw | ConvertFrom-Json
$map = @($parsedMap)
if ($map.Count -ne 56) { throw "Expected 56 scene mappings; got $($map.Count). No changes made." }
$settings = Join-Path $root 'ProjectSettings/EditorBuildSettings.asset'
$loader = Join-Path $root 'Assets/Scripts/Assembly-CSharp/LevelManager.cs'
$buildText = [IO.File]::ReadAllText($settings)
$code = [IO.File]::ReadAllText($loader).Replace("`r`n", "`n")
$renames = @()
foreach ($entry in $map) {
    $old = Join-Path $root $entry.old
    $new = Join-Path $root $entry.new
    $hasOld = Test-Path -LiteralPath $old
    $hasNew = Test-Path -LiteralPath $new
    if ($hasOld -eq $hasNew) { throw "Missing or conflicting scene: $($entry.name). No changes made." }
    if ($hasOld) {
        if (!(Test-Path -LiteralPath ($old + '.meta')) -or (Test-Path -LiteralPath ($new + '.meta'))) {
            throw "Missing or conflicting meta: $($entry.name). No changes made."
        }
        $renames += [PSCustomObject]@{Old=$old; New=$new; Relative=$entry.old}
    } elseif (!(Test-Path -LiteralPath ($new + '.meta')) -or (Test-Path -LiteralPath ($old + '.meta'))) {
        throw "Unexpected meta state: $($entry.name). No changes made."
    }
    # Keep enabled flags, build order and any Unity 5 GUID fields intact.
    $buildText = $buildText.Replace($entry.old, $entry.new)
    if (!$buildText.Contains($entry.new)) { throw "Build Settings missing $($entry.name). No changes made." }
}
# Match the unchanged crypto lines, independent of API Updater's scene-name API.
$oldRead = "`t`tloadedLevelName = loadedLevelName.Replace(`"#`", `"/`");`n`t`treturn Utils.Decrypt(loadedLevelName);"
$newRead = "`t`treturn loadedLevelName; // Recovered readable scene names."
$oldWrite = "`t`tstring text = Utils.Encrypt(name);`n`t`treturn text.Replace(`"/`", `"#`");"
$newWrite = "`t`treturn name; // Recovered readable scene names."
foreach ($pair in @(@($oldRead, $newRead), @($oldWrite, $newWrite))) {
    if ($code.Contains($pair[1])) { continue }
    if ([regex]::Matches($code, [regex]::Escape($pair[0])).Count -ne 1) {
        throw 'Unexpected LevelManager source. No changes made; send LevelManager.cs for review.'
    }
    $code = $code.Replace($pair[0], $pair[1])
}
if ($renames.Count -eq 0 -and $code -eq [IO.File]::ReadAllText($loader).Replace("`r`n", "`n") -and $buildText -eq [IO.File]::ReadAllText($settings)) {
    Write-Host 'Scene names already restored.'
    return
}
# Complete backups before modifying anything. No duplicate scripts under Assets.
$backup = Join-Path $root ('RecoveryBackups/SceneNames-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $backup | Out-Null
Copy-Item -LiteralPath $settings -Destination (Join-Path $backup 'EditorBuildSettings.asset')
Copy-Item -LiteralPath $loader -Destination (Join-Path $backup 'LevelManager.cs')
foreach ($item in $renames) {
    $dest = Join-Path $backup $item.Relative
    New-Item -ItemType Directory -Force -Path (Split-Path $dest) | Out-Null
    Copy-Item -LiteralPath $item.Old -Destination $dest
    Copy-Item -LiteralPath ($item.Old + '.meta') -Destination ($dest + '.meta')
}
$done = New-Object System.Collections.ArrayList
try {
    foreach ($item in $renames) {
        Move-Item -LiteralPath $item.Old -Destination $item.New
        [void]$done.Add(@($item.Old, $item.New))
        Move-Item -LiteralPath ($item.Old + '.meta') -Destination ($item.New + '.meta')
        [void]$done.Add(@(($item.Old + '.meta'), ($item.New + '.meta')))
    }
    $utf8 = New-Object System.Text.UTF8Encoding($false)
    [IO.File]::WriteAllText($settings, $buildText, $utf8)
    [IO.File]::WriteAllText($loader, $code, $utf8)
} catch {
    $failure = $_
    for ($i = $done.Count - 1; $i -ge 0; $i--) {
        Move-Item -LiteralPath $done[$i][1] -Destination $done[$i][0]
    }
    Copy-Item -LiteralPath (Join-Path $backup 'EditorBuildSettings.asset') -Destination $settings -Force
    Copy-Item -LiteralPath (Join-Path $backup 'LevelManager.cs') -Destination $loader -Force
    throw $failure
}
Write-Host 'Restored all 56 scene names. Open Assets/Levels/AwakeScene.unity for the client entry point.'
Write-Host "Backups: $backup"
Write-Host 'This restores names and loader mappings, not Android services or UI shaders.'
