<#
.SYNOPSIS
Build / run helper for Media Archiver (Tauri + SvelteKit).

.DESCRIPTION
Installs npm dependencies and builds the Tauri application. Can optionally clean
previous build artifacts first, and launch the app instead of only building.

.PARAMETER Clean
Remove previous build artifacts (frontend `build/`, `.svelte-kit/`, and the Rust
`target/` via `cargo clean`) before doing anything else.

.PARAMETER Run
Launch the app instead of only building. By default this runs the app in
development mode (`npm run tauri dev`) with hot reload. Combine with -Release to
build a production bundle and launch the compiled executable instead.

.PARAMETER Release
Only meaningful together with -Run: build the release bundle and then start the
compiled .exe. Without -Run a plain build is already a release build, so this is
ignored.

.PARAMETER SkipInstall
Skip `npm install` (useful for quick re-runs when dependencies are unchanged).

.EXAMPLE
.\build.ps1                 # install deps + build release installers
.\build.ps1 -Run           # install deps + run the app (dev mode, hot reload)
.\build.ps1 -Clean -Run    # clean, then run the app (dev mode)
.\build.ps1 -Clean         # clean, then build release installers
.\build.ps1 -Release -Run  # build the release bundle, then launch the built exe
#>

[CmdletBinding()]
param(
    [switch]$Clean,
    [switch]$Run,
    [switch]$Release,
    [switch]$SkipInstall
)

$ErrorActionPreference = 'Stop'
# Always operate from the repository root (where this script lives) so the
# script works regardless of the caller's current directory.
Set-Location -LiteralPath $PSScriptRoot

function Assert-LastExit([string]$What) {
    if ($LASTEXITCODE -ne 0) {
        Write-Error "$What failed (exit code $LASTEXITCODE)."
        exit $LASTEXITCODE
    }
}

if ($Clean) {
    Write-Host "Cleaning build artifacts..." -ForegroundColor Green
    foreach ($dir in @('build', '.svelte-kit')) {
        if (Test-Path -LiteralPath $dir) {
            Remove-Item -LiteralPath $dir -Recurse -Force
        }
    }
    if (Test-Path -LiteralPath 'src-tauri/Cargo.toml') {
        cargo clean --manifest-path src-tauri/Cargo.toml
        Assert-LastExit 'cargo clean'
    }
}

if (-not $SkipInstall) {
    Write-Host "Installing dependencies..." -ForegroundColor Green
    npm install
    Assert-LastExit 'npm install'
}

# Dev-mode run: builds a debug app and launches it with hot reload. This is a
# blocking, long-running command that returns when the app window is closed.
if ($Run -and -not $Release) {
    Write-Host "Launching app in development mode (close the window to stop)..." -ForegroundColor Green
    npm run tauri dev
    Assert-LastExit 'npm run tauri dev'
    exit 0
}

# Release build (the default, and the path for `-Release -Run`).
Write-Host "Building Tauri application (release)..." -ForegroundColor Green
npm run tauri build
Assert-LastExit 'npm run tauri build'
Write-Host "Build completed successfully!" -ForegroundColor Green

if ($Run) {
    Write-Host "Locating built executable..." -ForegroundColor Green
    $exe = Get-ChildItem -Path 'src-tauri/target/release' -Filter '*.exe' -File -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -notmatch 'setup|installer|uninstall' } |
        Select-Object -First 1
    if ($null -eq $exe) {
        Write-Error "Build succeeded but no executable was found under src-tauri/target/release."
        exit 1
    }
    Write-Host "Starting $($exe.FullName)" -ForegroundColor Green
    Start-Process -FilePath $exe.FullName
}
