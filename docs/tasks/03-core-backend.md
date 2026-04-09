# Task 3: Core Backend & Metadata Extraction

## Objective
Implement the Rust core for spawning child processes, specifically focusing on extracting metadata using `yt-dlp`.

## Context
The Rust backend acts as the bridge to OS-level operations and CLI engines (`docs/02-architecture.md`). It needs to parse complex JSON from `yt-dlp` into structured data (`docs/05-data-model.md`).

## Steps
1. **Data Models:**
   - Create Rust structs mirroring the `MediaMetadata` and `MediaFormat` interfaces defined in `docs/05-data-model.md`.
   - Implement `serde` for serialization/deserialization.

2. **Metadata Extraction Command:**
   - Create a Tauri command `fetch_metadata(url: String)`.
   - Use `std::process::Command` (or `tokio::process::Command` for async) to execute: `<path_to_ytdlp> --dump-json <URL>`.
   - Capture the `stdout` output.
   - Deserialize the JSON output into the Rust `MediaMetadata` struct.
   - Handle errors (e.g., invalid URL, network issues, unsupported site).

3. **Process Manager Foundation:**
   - Set up a basic async process manager that can spawn child processes and capture output streams. This will be extended later for actual downloads.

## Completion Criteria
- Calling `fetch_metadata` from the Svelte frontend successfully returns a fully populated `MediaMetadata` object for a valid YouTube/Vimeo URL.
- Appropriate error messages are returned for invalid URLs.
