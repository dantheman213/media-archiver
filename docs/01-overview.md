# Media Archiver - Overview

## Product Vision
"Media Archiver" is a premium, cross-platform desktop application built with Tauri, Rust, Svelte, and Vite. It serves as a powerful yet highly intuitive media downloading and archiving tool. Instead of exposing the raw, complex CLI arguments of its underlying engines (`yt-dlp` and `ffmpeg`), Media Archiver interprets user intent and provides a seamless, workflow-driven experience.

The goal is to make a highly capable archiving tool feel like a top-tier native application, accessible to non-technical users while retaining the power features professionals need.

## Core Workflows
1. **Media Ingestion:** Users paste URLs or drag-and-drop links into the app. The application immediately fetches rich metadata (titles, descriptions, high-resolution thumbnails, and available formats) in the background before any actual downloading begins.
2. **Workflow Configuration:** Users select human-readable actions such as "Extract High-Quality Audio", "Download Best Video for Mobile", or "Archive Complete Playlist". Under the hood, these map to optimized `yt-dlp` and `ffmpeg` arguments.
3. **Batch Processing:** Users can queue multiple links or entire playlists, adjust settings per individual item or apply global configurations, and start a batch process.
4. **Execution & Monitoring:** A robust, visually appealing progress view displays real-time download speeds, conversion status, and overall batch progress, providing clear feedback at every step.

## Key Principles
- **Top-Tier UI/UX:** The application must feel native, responsive, and aesthetically pleasing. Consistent spacing, smooth transitions, and intuitive feedback are required.
- **Not Just a Wrapper:** Abstract away CLI complexity. Use intuitive controls (toggles, sliders, dropdowns) instead of text inputs for arguments.
- **Bring Your Own Binaries (BYOB):** The application does not bundle `yt-dlp` or `ffmpeg` to avoid bloat and compliance issues. Instead, it provides a seamless onboarding experience to either download them automatically from official sources or link existing local installations.