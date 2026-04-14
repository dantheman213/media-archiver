import { writable, get } from 'svelte/store';
import { invoke } from '@tauri-apps/api/core';
import type { MediaMetadata } from '../types';

// ---------------------------------------------------------------------------
// Metadata cache – avoids re-fetching yt-dlp JSON for recently seen URLs
// ---------------------------------------------------------------------------

const STORAGE_KEY = 'metadata_cache';
const DEFAULT_TTL_HOURS = 24;

interface CachedEntry {
  metadata: MediaMetadata;
  cachedAt: string; // ISO 8601
}

const cache = writable<Record<string, CachedEntry>>({});

// ---------------------------------------------------------------------------
// Persistence helpers
// ---------------------------------------------------------------------------

async function persist(): Promise<void> {
  try {
    const data = get(cache);
    await invoke('save_json', { key: STORAGE_KEY, value: JSON.stringify(data) });
  } catch (e) {
    console.error('Failed to persist metadata cache:', e);
  }
}

// ---------------------------------------------------------------------------
// Public API
// ---------------------------------------------------------------------------

/** Load cache from disk on app startup */
export async function loadMetadataCache(): Promise<void> {
  try {
    const raw: string | null = await invoke('load_json', { key: STORAGE_KEY });
    if (raw) {
      const data: Record<string, CachedEntry> = JSON.parse(raw);
      cache.set(data);
    }
  } catch (e) {
    console.error('Failed to load metadata cache:', e);
  }
}

/** Get cached metadata for a URL (returns null if missing or expired) */
export function getCachedMetadata(url: string): MediaMetadata | null {
  const data = get(cache);
  const entry = data[url];
  if (!entry) return null;

  // Check TTL
  const cachedAt = new Date(entry.cachedAt).getTime();
  const now = Date.now();
  const ageHours = (now - cachedAt) / (1000 * 60 * 60);
  if (ageHours > DEFAULT_TTL_HOURS) {
    // Expired – remove it
    cache.update(d => {
      const copy = { ...d };
      delete copy[url];
      return copy;
    });
    return null;
  }

  return entry.metadata;
}

/** Store metadata for a URL in the cache and persist */
export async function cacheMetadata(url: string, metadata: MediaMetadata): Promise<void> {
  cache.update(d => ({
    ...d,
    [url]: {
      metadata,
      cachedAt: new Date().toISOString(),
    },
  }));
  await persist();
}

/** Clear the entire metadata cache */
export async function clearMetadataCache(): Promise<void> {
  cache.set({});
  try {
    await invoke('delete_json', { key: STORAGE_KEY });
  } catch (e) {
    console.error('Failed to delete metadata cache:', e);
  }
}
