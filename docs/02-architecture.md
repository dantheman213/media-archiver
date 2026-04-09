# Architecture

## Tech Stack
- **Frontend:** Svelte + Vite + TypeScript. Vanilla CSS will be used for styling to ensure maximum flexibility and a custom, polished aesthetic without the overhead of heavy UI frameworks.
- **Backend:** Rust (via Tauri), providing a lightweight, memory-safe, and highly performant core.
- **Engines:** `yt-dlp` and `ffmpeg` (managed as external, dynamically linked executables).

## System Components

### 1. Tauri Rust Core
The Rust backend is responsible for all heavy lifting and OS-level interactions.
- **Command Interface:** Exposes asynchronous Rust functions to the Svelte frontend (e.g., `fetch_metadata`, `start_download`, `pause_download`, `manage_binaries`).
- **Process Manager:** Spawns and monitors `yt-dlp` and `ffmpeg` child processes. It reads `stdout` and `stderr` streams in real-time to parse progress percentages, speeds, and errors, emitting structured events back to the frontend.
- **File System/OS:** Handles saving files, managing application state (JSON/SQLite), and interacting with the OS for native file save dialogues and notifications.

### 2. Svelte Frontend
The frontend handles presentation and user interaction.
- **State Management:** Svelte stores are utilized to manage the global download queue, active job statuses, and application settings.
- **UI Components:** Reusable, highly polished components for media cards, progress bars, workflow selectors, and settings panels.

### 3. Engine Bridge
A dedicated Rust module that translates declarative UI intents into precise CLI arguments.
- **Intent Translation:** Translates "Audio Only, MP3, Highest Quality" into `--extract-audio --audio-format mp3 --audio-quality 0`.
- **JSON Parsing:** Uses `yt-dlp --dump-json` to extract raw metadata and serializes it into Rust structs before passing it to the frontend.