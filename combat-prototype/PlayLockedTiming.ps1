param(
    [string]$GodotPath = (Join-Path $PSScriptRoot 'Godot_v4.7.2-stable_mono_win64.exe'),
    [switch]$LatencyDuel
)

$ErrorActionPreference = 'Stop'
$godot = (Resolve-Path -LiteralPath $GodotPath).Path
& dotnet build (Join-Path $PSScriptRoot 'CombatPrototype.csproj') --nologo | Out-Host
if ($LASTEXITCODE -ne 0) { throw 'Combat Prototype build failed.' }

if ($LatencyDuel) {
    Start-Process -FilePath $godot -WorkingDirectory $PSScriptRoot -ArgumentList '--path', ('"{0}"' -f $PSScriptRoot), 'res://LatencyDuel.tscn' -PassThru
    return
}

$session = (New-Item -ItemType Directory -Path (Join-Path ([IO.Path]::GetTempPath()) (
    'combat-locked-timing-' + [guid]::NewGuid().ToString('N')))).FullName
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'profiles/locked-timing.json') -Destination (Join-Path $session 'opponent.json')

$previous = $env:COMBAT_LIVE_DIR
try {
    $env:COMBAT_LIVE_DIR = $session
    $game = Start-Process -FilePath $godot -WorkingDirectory $PSScriptRoot -ArgumentList '--path', ('"{0}"' -f $PSScriptRoot) -PassThru
    [pscustomobject]@{ pid = $game.Id; dir = $session }
}
finally {
    $env:COMBAT_LIVE_DIR = $previous
}
