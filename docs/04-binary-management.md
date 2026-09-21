# Binary Management

## The Challenge
Bundling `yt-dlp` and `ffmpeg` directly into the application installer leads to bloated app sizes, potential licensing/compliance issues (especially with `ffmpeg`), and critically, prevents users from quickly updating `yt-dlp` when website extractors inevitably break.

## The "Bring Your Own Binaries" (BYOB) Solution
Media Archiver uses a dynamic, user-guided binary management system.

### Onboarding Experience
On first launch, the app checks for existing binaries in the system PATH or the app's local data directory. If missing, a full-screen, friendly prompt offers two choices:

1. **Auto-Install (Recommended):** 
   - The prompt lists exactly which dependencies will be installed (`yt-dlp`, `ffmpeg`, `ffprobe`, `AtomicParsley`) and the URL each one is downloaded from.
   - The app securely downloads the latest `yt-dlp` executable directly from its official GitHub releases.
   - It downloads a native `ffmpeg`/`ffprobe` build for the host CPU (Windows x64/arm64, macOS x64/arm64). On Apple Silicon this avoids the Intel-only builds that fail without Rosetta.
   - On Linux, instead of a prebuilt static binary it installs `ffmpeg`/`ffprobe` from the distro's own package manager (apt, dnf/yum, pacman, zypper, apk, xbps), elevating through `pkexec` for a graphical password prompt. Native packages match the system C library, avoiding glibc mismatches.
   - Progress bars are shown for these initialization downloads.
2. **Manual Setup (Advanced):** 
   - The user browses their file system to select the paths to existing `yt-dlp` and `ffmpeg` executables (`ffprobe` is detected alongside `ffmpeg`).

### Managing & Updating Binaries
Dependency status, per-binary path overrides, and updates all live in **Settings → Dependencies**.
- **Status:** Each dependency shows whether it was found, its version, and its resolved path.
- **Path overrides (BYOB):** Any dependency can be pointed at a user-supplied executable; clearing the override returns it to auto-detection (managed copy, then system PATH). `ffprobe` falls back to the copy beside the resolved `ffmpeg`.
- **yt-dlp updates:** "Check for updates" only checks (via the GitHub API) and, when a newer release exists, shows the new version as a green label beside the current one and reveals an "Update yt-dlp" button. The button runs `yt-dlp -U`.
- **ffmpeg:** Rarely needs updating for basic functionality, but the app verifies the version and can report it.

### Sandboxing & Permissions
The Rust backend ensures that executables downloaded via Auto-Install are placed in the correct OS-specific AppData/Local application directories and have the appropriate execution permissions (e.g., `chmod +x` on macOS/Linux).