# Task 4: UI Layout & State Management

## Objective
Establish the foundational UI layout and global state management using Svelte.

## Context
The app requires a modern two-pane layout and complex state management for the media queue (`docs/03-ui-ux.md`, `docs/05-data-model.md`).

## Steps
1. **TypeScript Interfaces:**
   - Create `src/types/index.ts` and define `MediaJob`, `JobStatus`, `MediaMetadata`, `MediaFormat`, `JobConfig`, and `JobProgress` exactly as outlined in `docs/05-data-model.md`.

2. **Svelte Stores:**
   - Create `src/stores/queue.ts` containing a writable store for the array of `MediaJob`s.
   - Create functions to add jobs, update job status, and update job progress.
   - Create `src/stores/settings.ts` for application settings (e.g., default paths, theme, concurrent downloads, auto-retry, global pause state).

3. **Main Layout Component:**
   - Build a two-pane layout: a fixed Sidebar and a scrollable Main Content Area.
   - **Sidebar:** Add navigation links for "Queue", "History", "Settings", and "Binaries Status".
   - **Main Area:** Create placeholder views for the active navigation route.

4. **Styling:**
   - Apply clean, modern CSS. Ensure responsive behavior (e.g., the sidebar can be collapsed or has a fixed width).
   - Use CSS variables for consistent theming.

5. **Keyboard Shortcuts:**
   - Implement global keydown listeners to handle `Spacebar` for global pause/resume and `Delete` for removing selected items.

## Completion Criteria
- Application has a navigable sidebar and main content area.
- TypeScript types are fully defined and integrated.
- Svelte stores are set up and can be imported into components.
