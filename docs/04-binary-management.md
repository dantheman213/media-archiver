# Binary Management

## The Challenge
Bundling `yt-dlp` and `ffmpeg` directly into the application installer leads to bloated app sizes, potential licensing/compliance issues (especially with `ffmpeg`), and critically, prevents users from quickly updating `yt-dlp` when website extractors inevitably break.

## The "Bring Your Own Binaries" (BYOB) Solution
Media Archiver uses a dynamic, user-guided binary management system.

### Onboarding Experience
On first launch, the app checks for existing binaries in the system PATH or the app's local data directory. If missing, a full-screen, friendly prompt offers two choices:

1. **Auto-Install (Recommended):** 
   - The app securely downloads the latest `yt-dlp` executable directly from its official GitHub releases.
   - It downloads a native `ffmpeg`/`ffprobe` build for the host CPU (Windows x64/arm64, macOS x64/arm64). On Apple Silicon this avoids the Intel-only builds that fail without Rosetta.
   - On Linux, instead of a prebuilt static binary it installs `ffmpeg`/`ffprobe` from the distro's own package manager (apt, dnf/yum, pacman, zypper, apk, xbps), elevating through `pkexec` for a graphical password prompt. Native packages match the system C library, avoiding glibc mismatches.
   - Progress bars are shown for these initialization downloads.
2. **Manual Setup (Advanced):** 
   - The user browses their file system to select the paths to existing `yt-dlp` and `ffmpeg` executables.

### Updating Binaries
`yt-dlp` requires frequent updates to maintain compatibility with media sites.
- **Dedicated Update UI:** The settings or sidebar features an "Update Engines" section indicating the current versions.
- **yt-dlp:** Clicking update triggers `yt-dlp -U` (its built-in update mechanism). If auto-installed, it can alternatively fetch the latest release via the GitHub API.
- **ffmpeg:** Rarely needs updating for basic functionality, but the app can verify the version and offer to download a newer static build if desired.

### Sandboxing & Permissions
The Rust backend ensures that executables downloaded via Auto-Install are placed in the correct OS-specific AppData/Local application directories and have the appropriate execution permissions (e.g., `chmod +x` on macOS/Linux).