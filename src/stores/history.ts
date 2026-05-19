import { writable, get } from 'svelte/store';
import { invoke } from '@tauri-apps/api/core';
import type { HistoryRecord } from '../types';

// ---------------------------------------------------------------------------
// History store – persistent record of completed downloads
// ---------------------------------------------------------------------------

const STORAGE_KEY = 'history';

export const history = writable<HistoryRecord[]>([]);

// ---------------------------------------------------------------------------
// Persistence helpers
// ---------------------------------------------------------------------------

async function persist(): Promise<void> {
  try {
    const records = get(history);
    await invoke('save_json', { key: STORAGE_KEY, value: JSON.stringify(records) });
  } catch (e) {
    console.error('Failed to persist history:', e);
  }
}

// ---------------------------------------------------------------------------
// Public API
// ---------------------------------------------------------------------------

/** Load history from disk on app startup */
export async function loadHistory(): Promise<void> {
  try {
    const raw: string | null = await invoke('load_json', { key: STORAGE_KEY });
    if (raw) {
      const records: HistoryRecord[] = JSON.parse(raw);
      history.set(records);
    }
  } catch (e) {
    console.error('Failed to load history:', e);
  }
}

/** Cache thumbnails for the most recent N records — call at startup so History looks instant */
export async function preloadRecentThumbnails(count = 10): Promise<void> {
  const records = get(history);
  const needsCache = records
    .filter(r => r.thumbnailUrl && !r.cachedThumbnailPath)
    .slice(0, count);
  await Promise.all(
    needsCache.map(async (r) => {
      try {
        const localPath: string = await invoke('cache_thumbnail', { url: r.thumbnailUrl, jobId: r.id });
        await updateHistoryRecord(r.id, { cachedThumbnailPath: localPath });
      } catch {
        // Non-critical
      }
    })
  );
}

/** Add a completed download to history and persist */
export async function addHistoryRecord(record: HistoryRecord): Promise<void> {
  history.update(current => [record, ...current]);
  await persist();
}

/** Remove a single history record by id and persist */
export async function removeHistoryRecord(id: string): Promise<void> {
  history.update(current => current.filter(r => r.id !== id));
  await persist();
}

/** Patch fields on an existing history record and persist */
export async function updateHistoryRecord(id: string, patch: Partial<HistoryRecord>): Promise<void> {
  history.update(current =>
    current.map(r => r.id === id ? { ...r, ...patch } : r)
  );
  await persist();
}

/** Clear all history records and persist */
export async function clearHistory(): Promise<void> {
  history.set([]);
  try {
    await invoke('delete_json', { key: STORAGE_KEY });
  } catch (e) {
    console.error('Failed to delete history file:', e);
  }
}
