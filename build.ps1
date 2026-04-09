<#
.SYNOPSIS
Build script for Media Archiver.

.DESCRIPTION
This script installs the necessary npm dependencies and then builds the Tauri application for Windows.
#>

Write-Host "Installing dependencies..." -ForegroundColor Green
npm install

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to install dependencies."
    exit $LASTEXITCODE
}

Write-Host "Building Tauri application..." -ForegroundColor Green
npm run tauri build

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build the Tauri application."
    exit $LASTEXITCODE
}

Write-Host "Build completed successfully!" -ForegroundColor Green
