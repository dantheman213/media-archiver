# UI/UX Design

## Layout Structure
The application uses a modern, clean, two-pane layout.

1. **Sidebar / Navigation:** 
   - Quick links to: **Queue** (active/pending), **History** (completed jobs), **Settings**, and **Binaries Status**.
2. **Main Content Area:**
   - **Input Zone:** A prominent, welcoming area at the top to paste URLs or drop links.
   - **Active Queue:** A list or grid of media items. Items start in a "Configuring" state before moving to the active batch.
3. **Inspector Panel (Collapsible):** 
   - When a media item is selected, a side panel slides in offering detailed, item-specific settings (format selection, trimming, subtitles).

## Humanizing CLI Options
The UI will abstract CLI arguments into intuitive controls:
- **Quality Presets:** Instead of exposing `-f bestvideo+bestaudio`, the UI offers plain-language presets like "Maximum Quality", "Balanced", and "Data Saver". Advanced users can enable an "Expert Mode" to select specific streams.
- **Format & Transcoding:** Users should not need to understand FFmpeg's `CRF` (Constant Rate Factor) or audio bitrates. Instead, present options like "Visually Lossless (High)", "Balanced (Medium)", and "Small File Size (Low)" which the backend maps to appropriate `-crf` values (e.g., 18, 23, 28) or yt-dlp `--audio-quality` levels (e.g., 0, 5, 9). Target formats (MP4, MKV, MP3, FLAC) are selectable via simple dropdowns.
- **Subtitles:** Instead of `--write-sub --sub-lang en`, the UI presents a "Subtitles" section with a language multi-select component and a toggle to "Embed into video file".
- **Trimming:** Provide a visual timeline or simple start/end time inputs that map directly to `ffmpeg -ss` and `-to` arguments.

## Metadata Capture & Presentation
When a user pastes a link:
1. The app immediately runs `yt-dlp --dump-json <URL>` in the background.
2. The UI creates a placeholder card with a loading skeleton.
3. Once data returns, the card populates with a rich thumbnail, video duration, title, channel/uploader name, and available formats.
4. **Crucially:** This happens *before* the item is added to the active download queue, allowing the user to make informed configuration decisions based on the actual media available.

**Fast Add Mode:** To save bandwidth and time when downloading batches, users can toggle a "Fast Add" mode in the input zone. When active, this bypasses the metadata fetching step entirely. The job immediately enters the active queue using the application's default preset configuration.

## Progress & Feedback
- **State Transitions:** Distinct visual states for "Inspecting" (fetching metadata), "Queued", "Downloading", "Converting" (ffmpeg post-processing), "Complete", and "Error".
- **Live Metrics:** Determinate progress bars parsed from `yt-dlp`'s output, displaying percentage, ETA, and download speed.
- **Animations:** Smooth transitions when items are added to the queue, complete their tasks, or are removed.

## Advanced UX Features
- **Error Recovery:** Failed jobs feature a "Retry" button. An "Auto-Retry" toggle in Settings automatically attempts to restart failed downloads up to 3 times.
- **Resource Management:** A global "Concurrent Downloads" slider (1-5) and a "Global Pause/Resume" button to control the entire queue.
- **Storage Awareness:** The Input Zone displays available disk space on the target drive. A warning appears if a batch's estimated size exceeds available space.
- **Power User Ergonomics:**
  - `Ctrl/Cmd + V`: Paste URL and trigger fetch.
  - `Delete`: Remove selected job.
  - `Spacebar`: Global Pause/Resume.
- **Post-Processing UX:** Completed jobs in History show quick actions: "Show in Explorer/Finder" and "Open in Default Media Player".