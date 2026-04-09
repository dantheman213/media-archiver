import { writable } from 'svelte/store';
import { invoke } from '@tauri-apps/api/core';
import type { GlobalSettings } from '../types';

// ---------------------------------------------------------------------------
// Settings store – application-wide configuration
// ---------------------------------------------------------------------------

const defaultSettings: GlobalSettings = {
  concurrentDownloads: 3,
  autoRetry: true,
  globalPaused: false,
  downloadPath: '',
  theme: 'system',
};

export const settings = writable<GlobalSettings>({ ...defaultSettings });

// ---------------------------------------------------------------------------
// Convenience mutators
// ---------------------------------------------------------------------------

/** Initialize settings (e.g., fetch OS default path) */
export async function initializeSettings(): Promise<void> {
  // If no path is set, we could ask Rust for a good default (the user's Downloads folder)
  // or we can let the backend handle it as a fallback. 
  // For the UI, it's nicer if the box shows the default.
  // We can't use @tauri-apps/api/path easily in a store during SSR or setup,
  // but we can call a Rust command.
  try {
    const defaultDir: string | null = await invoke('get_default_download_dir');
    if (defaultDir) {
      settings.update(s => {
        if (!s.downloadPath) return { ...s, downloadPath: defaultDir };
        return s;
      });
    }
  } catch (e) {
    console.error("Failed to initialize default download dir:", e);
  }
}

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
