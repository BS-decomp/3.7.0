param([Parameter(Mandatory=$true)][string]$ProjectPath)
$ErrorActionPreference = 'Stop'
$root = (Resolve-Path -LiteralPath $ProjectPath).Path
$patches = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'unity56-fixes.json') -Raw | ConvertFrom-Json
$pending = @()
foreach ($p in $patches) {
    $file = Join-Path $root ('Assets\Scripts\Assembly-CSharp\' + $p.file)
    $text = [IO.File]::ReadAllText($file).Replace("`r`n", "`n")
    if ($text.Contains($p.new)) {
        Write-Host "Already applied: $($p.file)"
        continue
    }
    $count = [regex]::Matches($text, [regex]::Escape($p.old)).Count
    if ($count -ne 1) {
        throw "Unexpected source in $($p.file). No files changed. Send this file for review."
    }
    $pending += [PSCustomObject]@{Path=$file; Name=$p.file; Text=$text.Replace($p.old,$p.new)}
}
if ($pending.Count -eq 0) { Write-Host 'Nothing to change.'; return }
# Backups outside Assets so Unity does not compile duplicate classes.
$backup = Join-Path $root ('RecoveryBackups\' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $backup | Out-Null
foreach ($item in $pending) { Copy-Item -LiteralPath $item.Path -Destination (Join-Path $backup $item.Name) }
$utf8 = New-Object System.Text.UTF8Encoding($false)
foreach ($item in $pending) {
    [IO.File]::WriteAllText($item.Path, $item.Text, $utf8)
    Write-Host "Fixed: $($item.Name)"
}
Write-Host "Backups: $backup"
