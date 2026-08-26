import type { JobConfig, HistoryRecord } from '../types';

// ---------------------------------------------------------------------------
// Settings summary – turns a download's config into a flat list of
// { label, value } rows for display on queue and history cards.
// ---------------------------------------------------------------------------

export interface SettingRow {
  label: string;
  value: string;
}

const qualityLabels: Record<string, string> = {
  best: 'Max',
  balanced: 'Recommended',
  small_size: 'Compact',
};

/** Normalize the quality half of a history formatLabel to the short label. */
function normalizeQuality(raw: string): string {
  const q = raw.trim();
  if (q === 'Maximum Quality') return 'Max';
  if (q === 'Compact') return 'Compact';
  return q || 'Recommended';
}

/** Build the settings rows from a live job config (queue cards). */
export function describeJobConfig(config: JobConfig): SettingRow[] {
  const rows: SettingRow[] = [];

  if (config.workflow === 'audio_only') {
    rows.push({ label: 'Type', value: 'Audio' });
    rows.push({ label: 'Format', value: (config.audioOnlyConfig?.format ?? 'mp3').toUpperCase() });
    rows.push({ label: 'Quality', value: qualityLabels[config.audioOnlyConfig?.quality ?? 'balanced'] });
  } else {
    rows.push({ label: 'Type', value: 'Video' });
    rows.push({ label: 'Format', value: (config.videoTranscode?.targetFormat ?? 'mp4').toUpperCase() });
    rows.push({ label: 'Quality', value: qualityLabels[config.videoTranscode?.quality ?? 'balanced'] });
  }

  const includes: string[] = [];
  if (config.embedSubtitles) includes.push('Subtitles');
  if (config.embedMetadata) includes.push('Metadata');
  if (config.embedThumbnail) includes.push('Thumbnail');
  if (includes.length) rows.push({ label: 'Includes', value: includes.join(', ') });

  const start = config.trim?.start?.trim();
  const end = config.trim?.end?.trim();
  if (start || end) {
    rows.push({ label: 'Trim', value: `${start || '0:00'} → ${end || 'end'}` });
  }

  return rows;
}

/** Build the settings rows from a saved history record (history cards). */
export function describeHistoryRecord(record: HistoryRecord): SettingRow[] {
  const rows: SettingRow[] = [];
  const [fmt = '', qLabel = ''] = record.formatLabel.split(' - ');

  rows.push({ label: 'Type', value: record.workflow === 'audio_only' ? 'Audio' : 'Video' });
  if (fmt) rows.push({ label: 'Format', value: fmt });
  if (qLabel) rows.push({ label: 'Quality', value: normalizeQuality(qLabel) });

  return rows;
}
