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

## License

MIT
