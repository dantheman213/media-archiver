# Task 5: Media Ingestion UI

## Objective
Build the user interface for adding URLs, displaying metadata, and configuring job settings.

## Context
This implements the "Inspecting" flow where users paste URLs and the app fetches rich data before adding it to the queue (`docs/01-overview.md`, `docs/03-ui-ux.md`).

## Steps
1. **Input Zone:**
   - Create a prominent input field at the top of the Queue view for pasting URLs.
   - (Optional) Implement drag-and-drop support for links.
   - Add a display for available disk space. Implement a warning if the estimated size (from metadata) is high.
   - Bind `Ctrl/Cmd + V` to focus the input and trigger the ingestion flow.

2. **Media Card Component:**
   - Create a `MediaCard.svelte` component that takes a `MediaJob`.
   - Implement a "Loading Skeleton" state for when `status === 'inspecting'`.
   - When metadata is loaded, display the thumbnail, title, duration, and uploader.

3. **The Ingestion Flow:**
   - On URL submit, create a new `MediaJob` in the store with status `inspecting`.
   - Call the `fetch_metadata` Tauri command.
   - Upon success, update the job in the store with status `configuring` and the returned `metadata`.

4. **Collapsible Inspector Panel:**
   - Build a side panel that slides in when a `MediaJob` in the `configuring` state is selected.
   - Create UI controls for `JobConfig`: Target Format (MP4, MKV, etc.), Video/Audio Quality presets ("Visually Lossless", "Balanced", "Small File Size" mapping to CRF behind the scenes), Audio-only toggle, Subtitles selection, and basic trimming inputs.
   - Add a button to "Confirm & Add to Queue".

## Completion Criteria
- Pasting a URL shows a loading card.
- The card updates with real thumbnail and details once metadata is fetched.
- Clicking the card opens a settings panel to configure the download.
- Confirming settings moves the job to the `queued` status.
