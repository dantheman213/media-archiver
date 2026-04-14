import { writable, get } from 'svelte/store';
import { invoke } from '@tauri-apps/api/core';
import type { GlobalSettings } from '../types';

// ---------------------------------------------------------------------------
// Settings store – application-wide configuration with persistence
// ---------------------------------------------------------------------------

const STORAGE_KEY = 'settings';

const defaultSettings: GlobalSettings = {
  concurrentDownloads: 3,
  autoRetry: true,
  globalPaused: false,
  downloadPath: '',
  theme: 'system',
};

export const settings = writable<GlobalSettings>({ ...defaultSettings });

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
      const current = get(settings);
      // Don't persist globalPaused — it's session-only state
      const { globalPaused, ...toPersist } = current;
      await invoke('save_json', { key: STORAGE_KEY, value: JSON.stringify(toPersist) });
    } catch (e) {
      console.error('Failed to save settings:', e);
    }
  }, 500);
}

// Subscribe to changes and auto-save
settings.subscribe(() => {
  scheduleSave();
});

// ---------------------------------------------------------------------------
// Initialization
// ---------------------------------------------------------------------------

/** Initialize settings – loads persisted values, then applies OS defaults */
export async function initializeSettings(): Promise<void> {
  // Load persisted settings
  try {
    const raw: string | null = await invoke('load_json', { key: STORAGE_KEY });
    if (raw) {
      const persisted = JSON.parse(raw);
      settings.update(s => ({
        ...s,
        ...persisted,
        // Always start unpaused
        globalPaused: false,
      }));
    }
  } catch (e) {
    console.error('Failed to load persisted settings:', e);
  }

  // If no download path is set, ask Rust for the OS default
  try {
    const current = get(settings);
    if (!current.downloadPath) {
      const defaultDir: string | null = await invoke('get_default_download_dir');
      if (defaultDir) {
        settings.update(s => ({ ...s, downloadPath: defaultDir }));
      }
    }
  } catch (e) {
    console.error('Failed to initialize default download dir:', e);
  }

  initialized = true;
}

// ---------------------------------------------------------------------------
// Convenience mutators
// ---------------------------------------------------------------------------

export function setTheme(theme: 'system' | 'light' | 'dark'): void {
  settings.update((s) => ({ ...s, theme }));
}

/** Toggle the global pause state */
export function toggleGlobalPause(): void {
  settings.update((s) => ({ ...s, globalPaused: !s.globalPaused }));
}

/** Set the number of concurrent downloads (clamped 1–5) */
export function setConcurrentDownloads(n: number): void {
  const clamped = Math.max(1, Math.min(5, n));
  settings.update((s) => ({ ...s, concurrentDownloads: clamped }));
}

/** Toggle auto-retry */
export function toggleAutoRetry(): void {
  settings.update((s) => ({ ...s, autoRetry: !s.autoRetry }));
}

/** Set the default download path */
export function setDownloadPath(path: string): void {
  settings.update((s) => ({ ...s, downloadPath: path }));
}

/** Reset all settings to defaults and clear persisted data */
export async function resetSettings(): Promise<void> {
  settings.set({ ...defaultSettings });
  try {
    await invoke('delete_json', { key: STORAGE_KEY });
  } catch (e) {
    console.error('Failed to delete persisted settings:', e);
  }
  // Re-fetch default download dir
  try {
    const defaultDir: string | null = await invoke('get_default_download_dir');
    if (defaultDir) {
      settings.update(s => ({ ...s, downloadPath: defaultDir }));
    }
  } catch (e) {
    console.error('Failed to re-initialize default download dir:', e);
  }
}
