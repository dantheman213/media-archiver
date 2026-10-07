import { invoke } from '@tauri-apps/api/core';
import { get } from 'svelte/store';
import {
  addJob,
  updateJobConfig,
  updateJobMetadata,
  updateJobStatus,
  defaultConfig,
  forceRedownload,
} from '../stores/queue';
import { downloadDefaults } from '../stores/downloadDefaults';
import { getCachedMetadata, cacheMetadata } from '../stores/metadataCache';
import type { HistoryRecord, JobConfig, MediaMetadata } from '../types';

export interface QueueUrlOptions {
  /** Force an overwrite of any existing file (used by Redownload). */
  force?: boolean;
  /** Called when the thumbnail has been cached locally for a job. */
  onThumbnailCached?: (jobId: string, localPath: string) => void;
}

/**
 * Queue a URL using the remembered download defaults (or a fresh default
 * config on first run), fetching metadata in the background so the card and
 * history fill in without blocking the download. Returns the new job id.
 */
export function queueUrlWithDefaults(url: string, options: QueueUrlOptions = {}): string {
  const cfg = get(downloadDefaults) ?? defaultConfig();
  // Add as 'inspecting' so the download waits for metadata: the selected
  // thumbnail's format decides whether yt-dlp can safely embed it (see
  // isThumbnailEmbeddable).
  const id = addJob(url);
  updateJobConfig(id, cfg);
  if (options.force) forceRedownload(id);

  const cached = getCachedMetadata(url);
  if (cached) {
    updateJobMetadata(id, cached);
    updateJobStatus(id, 'queued');
    return id;
  }

  invoke('fetch_metadata', { url })
    .then((metadata) => {
      updateJobMetadata(id, metadata as MediaMetadata);
      cacheMetadata(url, metadata as MediaMetadata);
      const thumb = (metadata as MediaMetadata).thumbnailUrl;
      if (thumb) {
        invoke('cache_thumbnail', { url: thumb, jobId: id })
          .then((localPath) => options.onThumbnailCached?.(id, localPath as string))
          .catch(() => { /* non-critical */ });
      }
    })
    .catch(() => { /* non-critical; download still proceeds */ })
    .finally(() => updateJobStatus(id, 'queued'));

  return id;
}

/**
 * Reconstruct the per-download options from a saved history record so a
 * redownload uses the same workflow/format/quality the user originally chose.
 */
export function configFromHistoryRecord(record: HistoryRecord): Partial<JobConfig> {
  const [fmtRaw = '', qLabelRaw = ''] = record.formatLabel.split(' - ');
  const fmt = fmtRaw.trim().toLowerCase();
  const quality = qLabelRaw === 'Maximum Quality' ? 'best' : qLabelRaw === 'Compact' ? 'small_size' : 'balanced';

  if (record.workflow === 'audio_only') {
    const formats = ['mp3', 'm4a', 'wav', 'flac'] as const;
    const audioFormat = (formats as readonly string[]).includes(fmt)
      ? (fmt as (typeof formats)[number])
      : 'mp3';
    return { workflow: 'audio_only', audioOnlyConfig: { format: audioFormat, quality } };
  }

  const videoFormats = ['mp4', 'mkv', 'webm'] as const;
  const targetFormat = (videoFormats as readonly string[]).includes(fmt)
    ? (fmt as (typeof videoFormats)[number])
    : 'mp4';
  return { workflow: 'video_best', videoTranscode: { targetFormat, quality } };
}