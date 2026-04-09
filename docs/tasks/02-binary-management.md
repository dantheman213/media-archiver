# Task 2: Binary Management (BYOB)

## Objective
Implement the "Bring Your Own Binaries" (BYOB) system for managing `yt-dlp` and `ffmpeg` executables.

## Context
To avoid bloat and compliance issues, the app must not bundle `yt-dlp` or `ffmpeg`. Instead, it needs a dynamic system to find, download, and manage them (`docs/04-binary-management.md`).

## Steps
1. **Rust Backend - Binary Detection:**
   - Create functions to check if `yt-dlp` and `ffmpeg` exist in the system `PATH`.
   - Create functions to check if they exist in the application's local data directory (e.g., `AppData/Local/MediaArchiver/bin` on Windows).

2. **Rust Backend - Auto-Install Logic:**
   - Implement a secure download mechanism to fetch the latest `yt-dlp` from official GitHub releases.
   - Implement a secure download mechanism to fetch OS-specific static builds of `ffmpeg`.
   - Ensure downloaded binaries are placed in the correct app data directory with execute permissions.
   - Implement progress tracking for these downloads via Tauri events.

3. **Frontend - Onboarding UI:**
   - Create an onboarding screen that appears on first launch if binaries are missing.
   - Present two options: "Auto-Install (Recommended)" and "Manual Setup (Advanced)".
   - For Auto-Install: Display progress bars for the initialization downloads.
   - For Manual Setup: Provide file picker inputs to select existing executable paths.

4. **Tauri Commands:**
   - Expose commands: `check_binaries`, `install_binaries`, `set_binary_paths`.

## Completion Criteria
- App successfully detects existing binaries or prompts the user.
- Auto-install correctly downloads and sets up `yt-dlp` and `ffmpeg`.
- Manual setup correctly saves and uses user-provided paths.
