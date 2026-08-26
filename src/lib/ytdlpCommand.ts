import type { MediaJob, HistoryRecord } from '../types';
import { sanitizeFilename } from './sanitizeFilename';

function quoteArg(s: string): string {
  return s.includes(' ') ? `"${s}"` : s;
}

/** Directory containing the ffmpeg binary (so yt-dlp also finds ffprobe). */
function ffmpegLocation(ffmpegPath: string): string {
  const dir = ffmpegPath.replace(/[/\\][^/\\]*$/, '');
  return dir || ffmpegPath;
}

export function buildCommandFromJob(
  job: MediaJob,
  defaultDownloadPath: string,
  useImpersonateChrome = true,
  useNoCookies = true,
  ytDlpPath?: string,
  ffmpegPath?: string,
): string {
  const outputPath = (job.config.downloadPath || defaultDownloadPath).replace(/[/\\]+$/, '');
  const parts: string[] = [quoteArg(ytDlpPath || 'yt-dlp')];

  const safeName = job.config.outputFilename ? sanitizeFilename(job.config.outputFilename) : '';
  const nameTemplate = safeName || '%(title)s';
  parts.push('-o', quoteArg(`${outputPath}/${nameTemplate}.%(ext)s`));
  parts.push('--windows-filenames');
  parts.push('--newline', '--progress');
  parts.push('--print', 'after_move:filepath');
  if (ffmpegPath) {
    parts.push('--ffmpeg-location', quoteArg(ffmpegLocation(ffmpegPath)));
  }

  // Collected into a single --postprocessor-args (repeated ones override).
  const ppArgs: string[] = [];

  if (job.config.workflow === 'audio_only') {
    parts.push('-x');
    const fmt = job.config.audioOnlyConfig?.format ?? 'mp3';
    parts.push('--audio-format', fmt);
    const q = job.config.audioOnlyConfig?.quality ?? 'balanced';
    const aq = q === 'best' ? '0' : q === 'balanced' ? '5' : '9';
    parts.push('--audio-quality', aq);
  } else {
    parts.push('-f', quoteArg('bestvideo+bestaudio/best'));
    const fmt = job.config.videoTranscode?.targetFormat ?? 'mp4';
    parts.push('--merge-output-format', fmt);
    const q = job.config.videoTranscode?.quality ?? 'balanced';
    const crf = q === 'best' ? '18' : q === 'balanced' ? '23' : '28';
    ppArgs.push(`-crf ${crf}`);
  }

  if (job.config.embedSubtitles) {
    parts.push('--embed-subs', '--sub-langs', 'all');
  }
  if (job.config.embedMetadata) {
    parts.push('--embed-metadata');
  }
  if (job.config.embedThumbnail) {
    parts.push('--embed-thumbnail');
  }

  // Trim via native section download (accurate, only fetches the range).
  const trimStart = job.config.trim?.start?.trim();
  const trimEnd = job.config.trim?.end?.trim();
  if (trimStart || trimEnd) {
    parts.push('--download-sections', quoteArg(`*${trimStart || '0'}-${trimEnd || 'inf'}`));
    parts.push('--force-keyframes-at-cuts');
  }

  if (ppArgs.length > 0) {
    parts.push('--postprocessor-args', quoteArg(ppArgs.join(' ')));
  }

  if (useImpersonateChrome) {
    parts.push('--impersonate', 'chrome');
  }
  if (useNoCookies) {
    parts.push('--no-cookies');
  }

  parts.push(quoteArg(job.url));
  return parts.join(' ');
}

export function buildCommandFromHistory(
  record: HistoryRecord,
  defaultDownloadPath: string,
  ytDlpPath?: string,
  ffmpegPath?: string,
): string {
  const outputPath = defaultDownloadPath.replace(/[/\\]+$/, '');
  const parts: string[] = [quoteArg(ytDlpPath || 'yt-dlp')];

  parts.push('-o', quoteArg(`${outputPath}/%(title)s.%(ext)s`));
  parts.push('--windows-filenames');
  parts.push('--newline', '--progress');
  parts.push('--print', 'after_move:filepath');
  if (ffmpegPath) {
    parts.push('--ffmpeg-location', quoteArg(ffmpegLocation(ffmpegPath)));
  }

  const [fmtRaw = '', qLabelRaw = ''] = record.formatLabel.split(' - ');
  const fmt = fmtRaw.toLowerCase();
  const q = qLabelRaw === 'Maximum Quality' ? 'best' : qLabelRaw === 'Compact' ? 'small_size' : 'balanced';

  if (record.workflow === 'audio_only') {
    parts.push('-x');
    parts.push('--audio-format', fmt || 'mp3');
    const aq = q === 'best' ? '0' : q === 'balanced' ? '5' : '9';
    parts.push('--audio-quality', aq);
  } else {
    parts.push('-f', quoteArg('bestvideo+bestaudio/best'));
    parts.push('--merge-output-format', fmt || 'mp4');
    const crf = q === 'best' ? '18' : q === 'balanced' ? '23' : '28';
    parts.push('--postprocessor-args', quoteArg(`-crf ${crf}`));
  }

  // History doesn't store embed options; use the same defaults as download.rs
  parts.push('--embed-metadata');
  parts.push('--embed-thumbnail');

  parts.push(quoteArg(record.url));
  return parts.join(' ');
}
