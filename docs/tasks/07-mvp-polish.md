# Task 7: MVP Polish & Error Handling

## Objective
Refine the application, implement error handling, and add binary update functionality to complete the MVP.

## Context
Ensure the application is robust, handles failures gracefully, and provides a premium feel (`docs/01-overview.md`, `docs/04-binary-management.md`).

## Steps
1. **Update Engines UI:**
   - In the Settings/Binaries view, display current versions of `yt-dlp` and `ffmpeg`.
   - Add an "Update yt-dlp" button.
   - Create a Rust command that runs `yt-dlp -U` (or fetches from GitHub if auto-installed) and reports success/failure.

2. **Global Error Handling:**
   - Catch Rust execution errors and emit them to the frontend.
   - Update `MediaJob` status to `error` and display a user-friendly error message on the card (e.g., "Video unavailable", "Network error").
   - Add a "Retry" button to the `MediaCard` for jobs in `error` state.
   - Add an auto-retry mechanism for failed jobs based on settings.

3. **UI/UX Polish:**
   - Refine CSS transitions (e.g., smooth sliding for the inspector panel, fade-ins for loaded images).
   - Ensure the "Queue" and "History" views correctly filter and display active vs completed jobs.
   - Implement OS-level notifications (using Tauri's notification API) when a batch completes.
   - For `completed` jobs in History, add quick action buttons to "Show in Explorer/Finder" and "Open in Default Media Player".

4. **End-to-End Testing:**
   - Test full workflows: Downloading a standard video, extracting high-quality audio, and handling a playlist.

## Completion Criteria
- User can successfully update `yt-dlp` from within the app.
- Errors are caught and displayed nicely without crashing the app.
- The UI feels smooth and polished.
- All core MVP workflows are fully functional.
