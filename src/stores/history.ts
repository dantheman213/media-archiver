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

/** Clear all history records and persist */
export async function clearHistory(): Promise<void> {
  history.set([]);
  try {
    await invoke('delete_json', { key: STORAGE_KEY });
  } catch (e) {
    console.error('Failed to delete history file:', e);
  }
}
