import { writable, get } from 'svelte/store';
import { invoke } from '@tauri-apps/api/core';
import type { JobConfig } from '../types';

// ---------------------------------------------------------------------------
// Download defaults store – remembers the last-used per-download options so
// they persist across videos AND across app restarts. Mirrors the persistence
// pattern in src/stores/settings.ts (debounced save via the Rust save_json
// command; loaded once on launch).
// ---------------------------------------------------------------------------

const STORAGE_KEY = 'download-defaults';

/**
 * The most recent "Configure Download" settings, reused by the default Add
 * button. Null until the user has configured at least one download (then the
 * default Add falls back to a fresh default config). The per-file rename
 * (`outputFilename`) and the per-video `trim` are intentionally never stored
 * here — they don't make sense to carry across different videos.
 */
export const downloadDefaults = writable<JobConfig | null>(null);

// ---------------------------------------------------------------------------
// Persistence – debounced save to disk via Rust backend
// ---------------------------------------------------------------------------

let saveTimeout: ReturnType<typeof setTimeout> | null = null;
let initialized = false;

function scheduleSave(): void {
  if (!initialized) return;
  if (saveTimeout) clearTimeout(saveTimeout);
  saveTimeout = setTimeout(async () => {
    try {
      const current = get(downloadDefaults);
      if (current === null) {
        await invoke('delete_json', { key: STORAGE_KEY });
      } else {
        await invoke('save_json', { key: STORAGE_KEY, value: JSON.stringify(current) });
      }
    } catch (e) {
      console.error('Failed to save download defaults:', e);
    }
  }, 500);
}

downloadDefaults.subscribe(() => {
  scheduleSave();
});

// ---------------------------------------------------------------------------
// Initialization
// ---------------------------------------------------------------------------

/** Load persisted download defaults (call once on launch). */
export async function initializeDownloadDefaults(): Promise<void> {
  try {
    const raw: string | null = await invoke('load_json', { key: STORAGE_KEY });
    if (raw) {
      const persisted = JSON.parse(raw) as JobConfig;
      // Defensively strip fields that must never carry across videos, in case
      // an older build persisted them.
      const { trim, outputFilename, ...clean } = persisted as JobConfig;
      downloadDefaults.set(clean as JobConfig);
    }
  } catch (e) {
    console.error('Failed to load persisted download defaults:', e);
  }
  initialized = true;
}

// ---------------------------------------------------------------------------
// Mutators
// ---------------------------------------------------------------------------

/**
 * Remember the given config as the new defaults, dropping the per-video and
 * per-file fields that should not be reused.
 */
export function rememberDownloadDefaults(config: JobConfig): void {
  const { trim, outputFilename, ...clean } = config;
  downloadDefaults.set(clean as JobConfig);
}
