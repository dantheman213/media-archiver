#!/bin/bash
#
# Build script for Media Archiver.
#
# This script installs the necessary npm dependencies and then builds
# the Tauri application for macOS.
#

set -e

echo "\033[0;32mInstalling dependencies...\033[0m"
npm install

echo "\033[0;32mBuilding Tauri application...\033[0m"
npm run tauri build

echo "\033[0;32mBuild completed successfully!\033[0m"
