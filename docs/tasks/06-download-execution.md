# Task 6: Download Execution & Progress Tracking

## Objective
Connect frontend configurations to the backend engines and display real-time download progress.

## Context
This is the core functional loop: translating user intent to CLI arguments, running the tools, parsing output, and updating the UI (`docs/01-overview.md`, `docs/02-architecture.md`).

## Steps
1. **Intent Translation (Rust):**
   - Create a Tauri command `start_download(job_id: String, url: String, config: JobConfig)`.
   - Write a Rust module that takes the `JobConfig` and generates the correct array of arguments for `yt-dlp` and `ffmpeg` (e.g., mapping quality presets like "Visually Lossless" to ffmpeg `-crf 18` and target format arguments, handling subtitle arguments).

2. **Process Execution & Monitoring (Rust):**
   - Implement a queue worker that respects the `concurrentDownloads` setting and handles `globalPaused` state.
   - Execute `yt-dlp` with the generated arguments.
   - Use `tokio` to asynchronously read the `stdout` and `stderr` streams of the child process.
   - Parse the standard `yt-dlp` progress output (e.g., `[download]  23.5% of 150.00MiB at  2.50MiB/s ETA 00:45`).
   - Emit Tauri events (e.g., `download-progress`) containing the parsed percentage, speed, and ETA, tagged with the `job_id`.

3. **Frontend Progress UI:**
   - In Svelte, listen to the `download-progress` Tauri event.
   - Update the relevant `MediaJob`'s `JobProgress` store.
   - Build a `ProgressBar.svelte` component to visually display percentage, speed, and ETA on the Media Card.
   - Update job status visually (`queued` -> `downloading` -> `processing` -> `completed`).

## Completion Criteria
- Initiating a download successfully runs `yt-dlp`.
- The UI reflects accurate, real-time progress.
- The downloaded file appears in the configured destination folder.
