# Media Archiver

A local media archiving application built with Tauri and SvelteKit.

## Description

Media Archiver is a desktop application designed to archive and manage media content locally. It leverages the power of Rust for its backend and SvelteKit for its frontend, providing a high-performance and modern user experience.

## Features

- Local media ingestion and management.
- Download execution for remote media sources.
- Comprehensive metadata management.
- User-friendly interface with SvelteKit.

## Prerequisites

- [Node.js](https://nodejs.org/) (latest LTS recommended)
- [Rust](https://www.rust-lang.org/) (and Cargo)
- C++ Build Tools (for Windows) - follow [Tauri's prerequisites](https://tauri.app/start/prerequisites/) for your platform.

## Development

To run the application in development mode:

```bash
npm run tauri dev
```

This will start the SvelteKit development server and the Tauri application window.

## Building the Application

To build the application, you can use the provided build script or run the commands manually.

### Using the build script (Windows)

Run the included PowerShell script:

```powershell
.\build.ps1
```

### Manual Build

1. Install the dependencies:

```bash
npm install
```

2. Build the Tauri application:

```bash
npm run tauri build
```

The resulting binaries will be available in the `src-tauri/target/release` directory.

## Releasing / Deployment

Versions are kept in sync across `package.json`, `src-tauri/Cargo.toml`, and `src-tauri/tauri.conf.json`
via an npm lifecycle hook. GitHub Actions builds and publishes a draft release whenever a `v*` tag is pushed.

**To cut a new release:**

```bash
# Bump version (choose one)
npm version patch   # 0.9.2 → 0.9.3
npm version minor   # 0.9.2 → 0.10.0
npm version major   # 0.9.2 → 1.0.0
npm version 1.2.3   # explicit version

# Push the commit and the generated tag
git push && git push --tags
```

This will:
1. Update the version in `package.json`, `Cargo.toml`, and `tauri.conf.json`
2. Create a git commit and a `v<version>` tag
3. Push to GitHub, triggering the Actions workflow
4. Build the Windows installer and publish a draft GitHub release

Approve the draft release on GitHub to make it public.

## Running Unsigned Builds

Release binaries are not code-signed, so macOS and Windows may warn you or block the app the first time you open it. The steps below let you bypass those warnings.

### macOS

After copying `Media Archiver.app` to your `Applications` folder, remove the quarantine attribute:

```bash
xattr -dr com.apple.quarantine "/Applications/Media Archiver.app"
```

Then open the app normally. Alternatively, right-click (or Control-click) the app and choose **Open** to allow it via Gatekeeper's per-app exception.

### Windows

Windows applies several layers of protection to unsigned apps:

**1. SmartScreen ("Windows protected your PC")**

- Click **More info**, then **Run anyway**.
- Or, before running, right-click the installer/executable → **Properties** → check **Unblock** → **OK**.

**2. Smart App Control (Windows 11)**

If Smart App Control is enabled, it may block the app without an override prompt. You can turn it off:

1. Open **Settings** → **Privacy & security** → **Windows Security** → **App & browser control**.
2. Under **Smart App Control**, set it to **Off**.

Note: Smart App Control can only be re-enabled by reinstalling Windows, so disable it only if you trust the app.

**3. Mark of the Web / Attachment Manager**

If the download came from the internet, unblock the file from PowerShell:

```powershell
Unblock-File -Path ".\Media Archiver_<version>_x64-setup.exe"
```

**4. Microsoft Defender antivirus false positives**

Unsigned builds are sometimes flagged by heuristic scanners. If Defender quarantines the file, restore it and add an exclusion for the install directory under **Windows Security** → **Virus & threat protection** → **Manage settings** → **Exclusions**.

## License

MIT
