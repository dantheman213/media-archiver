// Core type definitions for Media Archiver
// See docs/05-data-model.md for specification

export type JobStatus =
  | 'inspecting'   // Fetching JSON metadata
  | 'configuring'  // Waiting for user to confirm settings
  | 'queued'       // In the batch queue waiting to start
  | 'paused'       // Job is paused in queue
  | 'downloading'  // yt-dlp is downloading
  | 'processing'   // ffmpeg is merging/converting
  | 'completed'    // Done
  | 'error';       // Failed

export interface MediaFormat {
  formatId: string;
  ext: string;
  resolution: string;
  fps?: number;
  vcodec: string;
  acodec: string;
  filesize?: number;
  filesizeApprox?: number;
}

export interface MediaMetadata {
  title: string;
  thumbnailUrl: string;
  durationSeconds: number;
  uploader: string;
  description: string;
  extractor: string; // e.g., 'youtube', 'vimeo'
  availableFormats: MediaFormat[];
}

export interface JobConfig {
  workflow: 'video_best' | 'audio_only' | 'custom';
  format?: string; // specific format id if custom
  videoTranscode?: {
    targetFormat: 'mp4' | 'mkv' | 'webm';
    quality: 'best' | 'balanced' | 'small_size'; // Maps to CRF 18, 23, 28
  };
  audioOnlyConfig?: {
    format: 'mp3' | 'm4a' | 'wav' | 'flac';
    quality: 'best' | 'balanced' | 'small_size'; // Maps to yt-dlp audio-quality 0, 5, 9
  };
  embedSubtitles: boolean;
  subtitleLanguages: string[];
  embedMetadata: boolean;
  embedThumbnail: boolean;
  trim?: { start?: string; end?: string }; // 'HH:MM:SS'
}

export interface JobProgress {
  percentage: number;
  downloadSpeed: string; // e.g., '1.5MiB/s'
  eta: string;           // e.g., '00:01:30'
  currentStep: string;   // e.g., 'Downloading video stream...', 'Merging audio...'
}

export interface MediaJob {
  id: string; // UUID
  url: string;
  status: JobStatus;
  metadata: MediaMetadata | null; // Null if in Fast Add mode or still inspecting
  config: JobConfig;
  progress: JobProgress;
  errorMessage?: string;
}

export interface GlobalSettings {
  concurrentDownloads: number;
  autoRetry: boolean;
  globalPaused: boolean;
  downloadPath: string;
  theme: 'system' | 'light' | 'dark';
}

export type NavRoute = 'queue' | 'history' | 'settings' | 'binaries';
