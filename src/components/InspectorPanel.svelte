<script lang="ts">
  import { invoke } from '@tauri-apps/api/core';
  import { get } from 'svelte/store';
  import type { MediaJob, JobConfig } from '../types';
  import { updateJobConfig, updateJobStatus, removeJob } from '../stores/queue';
  import { rememberDownloadDefaults } from '../stores/downloadDefaults';
  import { binaryCheckState } from '../stores/binaries';
  import { settings } from '../stores/settings';
  import { isValidTimeString, parseTimeString, formatTime } from '../lib/timeUtils';
  import { sanitizeFilename } from '../lib/sanitizeFilename';
  import ToggleSwitch from './ToggleSwitch.svelte';
  import RangeSlider from './RangeSlider.svelte';

  let { job, onclose }: {
    job: MediaJob;
    onclose: () => void;
  } = $props();

  // Local mutable copies of the job config – synced when the job prop changes
  let workflow: JobConfig['workflow'] = $state('video_best');
  let targetFormat: 'mp4' | 'mkv' | 'webm' = $state('mp4');
  let videoQuality: 'best' | 'balanced' | 'small_size' = $state('balanced');
  let audioFormat: 'mp3' | 'm4a' | 'wav' | 'flac' = $state('mp3');
  let audioQuality: 'best' | 'balanced' | 'small_size' = $state('balanced');
  let embedSubtitles = $state(false);
  let embedMetadata = $state(true);
  let embedThumbnail = $state(true);
  let trimStart = $state('');
  let trimEnd = $state('');
  // Numeric trim bounds (seconds) that drive the dual-handle slider when the
  // media duration is known.
  let startSec = $state(0);
  let endSec = $state(0);
  let downloadPath = $state('');
  let outputFilename = $state('');

  // Whether we can show the slider (duration reported by yt-dlp).
  let hasDuration = $derived(!!job.metadata && job.metadata.durationSeconds > 0);
  let durationInt = $derived(Math.round(job.metadata?.durationSeconds ?? 0));

  // Sync local state when job changes
  $effect(() => {
    workflow = job.config.workflow;
    targetFormat = job.config.videoTranscode?.targetFormat ?? 'mp4';
    videoQuality = job.config.videoTranscode?.quality ?? 'balanced';
    audioFormat = job.config.audioOnlyConfig?.format ?? 'mp3';
    audioQuality = job.config.audioOnlyConfig?.quality ?? 'balanced';
    embedSubtitles = job.config.embedSubtitles;
    embedMetadata = job.config.embedMetadata;
    embedThumbnail = job.config.embedThumbnail;
    trimStart = job.config.trim?.start ?? '';
    trimEnd = job.config.trim?.end ?? '';
    // Seed slider bounds from any existing trim, else full range.
    const dur = Math.round(job.metadata?.durationSeconds ?? 0);
    startSec = parseTimeString(job.config.trim?.start ?? '') ?? 0;
    endSec = parseTimeString(job.config.trim?.end ?? '') ?? dur;
    downloadPath = job.config.downloadPath ?? get(settings).downloadPath;
    outputFilename = job.config.outputFilename ?? '';
  });

  // Live preview of the sanitized custom file name
  let sanitizedName = $derived(outputFilename.trim() ? sanitizeFilename(outputFilename) : '');

  // Human-friendly quality labels with descriptions
  const videoQualityOptions: { value: string; label: string; desc: string }[] = [
    { value: 'best', label: 'Maximum Quality', desc: 'Largest file, visually lossless' },
    { value: 'balanced', label: 'Recommended', desc: 'Great quality, reasonable size' },
    { value: 'small_size', label: 'Compact', desc: 'Smaller file, reduced quality' },
  ];

  const audioQualityOptions: { value: string; label: string; desc: string }[] = [
    { value: 'best', label: 'Maximum Quality', desc: 'Highest fidelity audio' },
    { value: 'balanced', label: 'Recommended', desc: 'Great quality, smaller file' },
    { value: 'small_size', label: 'Compact', desc: 'Smallest file, lower fidelity' },
  ];

  // Format descriptions
  const videoFormatDescs: Record<string, string> = {
    mp4: 'Most compatible \u2014 plays everywhere',
    mkv: 'Best for archiving \u2014 supports all features',
    webm: 'Open format \u2014 web optimized',
  };

  const audioFormatDescs: Record<string, string> = {
    mp3: 'Universal audio \u2014 works on all devices',
    m4a: 'Better quality than MP3 at the same size',
    wav: 'Uncompressed studio quality \u2014 very large',
    flac: 'Lossless compression \u2014 perfect quality, large files',
  };

  function formatDuration(seconds: number): string {
    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const s = Math.floor(seconds % 60);
    if (h > 0) return `${h}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
    return `${m}:${String(s).padStart(2, '0')}`;
  }

  async function browseSaveFolder() {
    const selected = await invoke<string | null>('select_directory');
    if (selected) downloadPath = selected;
  }

  function buildConfig(): Partial<JobConfig> {
    const cfg: Partial<JobConfig> = {
      workflow,
      downloadPath: downloadPath || undefined,
      outputFilename: outputFilename.trim() || undefined,
      embedSubtitles,
      embedMetadata,
      embedThumbnail,
    };

    if (workflow === 'video_best') {
      cfg.videoTranscode = {
        targetFormat,
        quality: videoQuality,
      };
    }

    if (workflow === 'audio_only') {
      cfg.audioOnlyConfig = {
        format: audioFormat,
        quality: audioQuality,
      };
    }

    // Trim: when the duration is known the slider is the source of truth
    // (a full-range selection means "no trim"); otherwise fall back to the
    // manually typed timestamps.
    let start = '';
    let end = '';
    if (hasDuration) {
      start = startSec > 0 ? formatTime(startSec) : '';
      end = endSec < durationInt ? formatTime(endSec) : '';
    } else {
      start = trimStart;
      end = trimEnd;
    }
    if (start || end) {
      cfg.trim = {
        ...(start ? { start } : {}),
        ...(end ? { end } : {}),
      };
    }

    return cfg;
  }

  function handleAddToQueue() {
    if ($binaryCheckState !== 'done') return;
    const cfg = buildConfig();
    updateJobConfig(job.id, cfg);
    updateJobStatus(job.id, 'queued');
    // Persist these settings as the remembered defaults for the quick Add
    // button and future app runs. Per-video trim and the per-file rename are
    // dropped inside rememberDownloadDefaults().
    rememberDownloadDefaults(cfg as JobConfig);
    onclose();
  }

  function handleRemove() {
    removeJob(job.id);
    onclose();
  }
</script>

<aside class="inspector-panel">
  <div class="inspector-header">
    <h3>Configure Download</h3>
    <button class="btn-close" onclick={onclose} aria-label="Close panel">&times;</button>
  </div>

  <div class="inspector-body">
    <!-- Media preview -->
    {#if job.metadata}
      <div class="preview-section">
        {#if job.metadata.thumbnailUrl}
          <img class="preview-thumb" src={job.metadata.thumbnailUrl} alt={job.metadata.title} />
        {/if}
        <h4 class="preview-title">{job.metadata.title}</h4>
        <div class="preview-meta">
          <span>{job.metadata.uploader}</span>
          {#if job.metadata.durationSeconds > 0}
            <span>{formatDuration(job.metadata.durationSeconds)}</span>
          {/if}
        </div>
      </div>
    {/if}

    <!-- Workflow selection -->
    <fieldset class="config-section">
      <legend>What to Save</legend>
      <div class="workflow-options">
        <label class="radio-card" class:active={workflow === 'video_best'}>
          <input type="radio" name="workflow" value="video_best" bind:group={workflow} />
          <span class="radio-label">Save Video</span>
          <span class="radio-desc">Download the best available video and audio</span>
        </label>
        <label class="radio-card" class:active={workflow === 'audio_only'}>
          <input type="radio" name="workflow" value="audio_only" bind:group={workflow} />
          <span class="radio-label">Save Audio</span>
          <span class="radio-desc">Extract just the audio track</span>
        </label>
      </div>
    </fieldset>

    <!-- Video options -->
    {#if workflow === 'video_best'}
      <fieldset class="config-section">
        <legend>Video Settings</legend>
        <div class="config-row">
          <label for="target-format">Format</label>
          <select id="target-format" bind:value={targetFormat}>
            <option value="mp4">MP4</option>
            <option value="mkv">MKV</option>
            <option value="webm">WebM</option>
          </select>
        </div>
        <div class="format-hint">{videoFormatDescs[targetFormat]}</div>
        <div class="config-row">
          <label for="video-quality">Quality</label>
          <select id="video-quality" bind:value={videoQuality}>
            {#each videoQualityOptions as opt}
              <option value={opt.value}>{opt.label}</option>
            {/each}
          </select>
        </div>
        <div class="format-hint">{videoQualityOptions.find(o => o.value === videoQuality)?.desc}</div>
      </fieldset>
    {/if}

    <!-- Audio options -->
    {#if workflow === 'audio_only'}
      <fieldset class="config-section">
        <legend>Audio Settings</legend>
        <div class="config-row">
          <label for="audio-format">Format</label>
          <select id="audio-format" bind:value={audioFormat}>
            <option value="mp3">MP3</option>
            <option value="m4a">M4A</option>
            <option value="wav">WAV</option>
            <option value="flac">FLAC</option>
          </select>
        </div>
        <div class="format-hint">{audioFormatDescs[audioFormat]}</div>
        <div class="config-row">
          <label for="audio-quality">Quality</label>
          <select id="audio-quality" bind:value={audioQuality}>
            {#each audioQualityOptions as opt}
              <option value={opt.value}>{opt.label}</option>
            {/each}
          </select>
        </div>
        <div class="format-hint">{audioQualityOptions.find(o => o.value === audioQuality)?.desc}</div>
      </fieldset>
    {/if}

    <!-- Embed options -->
    <fieldset class="config-section">
      <legend>Include in File</legend>
      <ToggleSwitch
        label="Subtitles"
        description="Include subtitle tracks inside the file"
        checked={embedSubtitles}
        onchange={(v) => embedSubtitles = v}
      />
      <ToggleSwitch
        label="Metadata"
        description="Save title, artist, and description in the file"
        checked={embedMetadata}
        onchange={(v) => embedMetadata = v}
      />
      <ToggleSwitch
        label="Thumbnail"
        description="Set the video thumbnail as cover art"
        checked={embedThumbnail}
        onchange={(v) => embedThumbnail = v}
      />
    </fieldset>

    <!-- Trim -->
    <fieldset class="config-section">
      <legend>Trim (optional)</legend>
      {#if hasDuration}
        <RangeSlider max={durationInt} bind:start={startSec} bind:end={endSec} />
        <div class="trim-slider-labels">
          <span>{formatTime(startSec)}</span>
          <span class="trim-total">Total {formatTime(durationInt)}</span>
          <span>{formatTime(endSec)}</span>
        </div>
        {#if startSec > 0 || endSec < durationInt}
          <div class="trim-summary">
            Trimming {formatTime(startSec)} &rarr; {formatTime(endSec)} ({formatTime(Math.max(0, endSec - startSec))} selected)
          </div>
        {:else}
          <div class="trim-hint">Full video &mdash; drag the handles to trim</div>
        {/if}
      {:else}
        <!-- Duration unknown (e.g. live streams): fall back to manual entry -->
        <div class="config-row">
          <label for="trim-start">Start</label>
          <input
            id="trim-start"
            type="text"
            placeholder="0:00"
            class:input-error={trimStart && !isValidTimeString(trimStart)}
            bind:value={trimStart}
          />
        </div>
        <div class="config-row">
          <label for="trim-end">End</label>
          <input
            id="trim-end"
            type="text"
            placeholder="0:00"
            class:input-error={trimEnd && !isValidTimeString(trimEnd)}
            bind:value={trimEnd}
          />
        </div>
        {#if trimStart && !isValidTimeString(trimStart)}
          <div class="trim-error">Use format MM:SS or HH:MM:SS</div>
        {:else if trimEnd && !isValidTimeString(trimEnd)}
          <div class="trim-error">Use format MM:SS or HH:MM:SS</div>
        {:else if trimStart && trimEnd && isValidTimeString(trimStart) && isValidTimeString(trimEnd)}
          {@const s = parseTimeString(trimStart)}
          {@const e = parseTimeString(trimEnd)}
          {#if s !== null && e !== null}
            {#if s >= e}
              <div class="trim-error">Start must be before end</div>
            {:else}
              <div class="trim-summary">Trimming from {trimStart} to {trimEnd} ({formatTime(e - s)} selected)</div>
            {/if}
          {/if}
        {/if}
      {/if}
    </fieldset>

    <!-- Output folder -->
    <fieldset class="config-section">
      <legend>Output Folder</legend>
      <div class="folder-row">
        <span class="folder-path" title={downloadPath}>{downloadPath || 'Default folder'}</span>
        <button class="btn-browse" onclick={browseSaveFolder}>Browse</button>
      </div>
    </fieldset>

    <!-- File name (optional rename) -->
    <fieldset class="config-section">
      <legend>File Name (optional)</legend>
      <input
        id="output-filename"
        type="text"
        placeholder="Keep original title"
        bind:value={outputFilename}
      />
      {#if sanitizedName && sanitizedName !== outputFilename.trim()}
        <div class="format-hint">Saved as: {sanitizedName}.&lt;ext&gt;</div>
      {:else}
        <div class="format-hint">The file extension is added automatically.</div>
      {/if}
    </fieldset>
  </div>

  <!-- Action buttons -->
  <div class="inspector-footer">
    <button class="btn-secondary" onclick={handleRemove}>Remove</button>
    <button class="btn-primary" onclick={handleAddToQueue} disabled={$binaryCheckState !== 'done'}>Add to Queue</button>
  </div>
</aside>

<style>
  .inspector-panel {
    width: 360px;
    min-width: 360px;
    height: 100%;
    background-color: var(--bg-sidebar);
    border-left: 1px solid var(--border-color);
    display: flex;
    flex-direction: column;
    overflow: hidden;
    animation: slide-in 0.2s ease-out;
  }

  @keyframes slide-in {
    from { transform: translateX(100%); }
    to { transform: translateX(0); }
  }

  .inspector-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: var(--spacing-md) var(--spacing-lg);
    border-bottom: 1px solid var(--border-color);
  }

  .inspector-header h3 {
    font-size: 1rem;
    font-weight: 700;
  }

  .btn-close {
    background: none;
    border: none;
    color: var(--text-muted);
    font-size: 1.4rem;
    cursor: pointer;
    padding: 0 var(--spacing-xs);
    line-height: 1;
  }

  .btn-close:hover {
    color: var(--text-color);
  }

  .inspector-body {
    flex: 1;
    overflow-y: auto;
    padding: var(--spacing-md) var(--spacing-lg);
    display: flex;
    flex-direction: column;
    gap: var(--spacing-lg);
  }

  /* Preview */
  .preview-section {
    display: flex;
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .preview-thumb {
    width: 100%;
    border-radius: var(--radius-md);
    aspect-ratio: 16 / 9;
    object-fit: cover;
    background-color: var(--bg-surface-hover);
  }

  .preview-title {
    font-size: 0.95rem;
    font-weight: 600;
    line-height: 1.3;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .preview-meta {
    display: flex;
    gap: var(--spacing-sm);
    font-size: var(--font-size-sm);
    color: var(--text-muted);
  }

  /* Config sections */
  .config-section {
    border: 1px solid var(--border-color);
    border-radius: var(--radius-md);
    padding: var(--spacing-md);
    display: flex;
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .config-section legend {
    font-size: var(--font-size-sm);
    font-weight: 600;
    color: var(--text-muted);
    padding: 0 var(--spacing-xs);
  }

  /* Workflow radio cards */
  .workflow-options {
    display: flex;
    flex-direction: column;
    gap: var(--spacing-xs);
  }

  .radio-card {
    display: flex;
    flex-direction: column;
    padding: var(--spacing-sm) var(--spacing-md);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-sm);
    cursor: pointer;
    transition: border-color 0.15s, background-color 0.15s;
  }

  .radio-card:hover {
    background-color: var(--bg-surface-hover);
  }

  .radio-card.active {
    border-color: var(--primary-color);
    background-color: rgba(0, 123, 255, 0.08);
  }

  .radio-card input[type="radio"] {
    display: none;
  }

  .radio-label {
    font-weight: 500;
    font-size: var(--font-size-sm);
  }

  .radio-desc {
    font-size: 0.75rem;
    color: var(--text-muted);
  }

  /* Config rows */
  .config-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: var(--spacing-sm);
  }

  .config-row label {
    font-size: var(--font-size-sm);
    color: var(--text-muted);
    flex-shrink: 0;
  }

  .config-row select,
  .config-row input[type="text"] {
    padding: var(--spacing-xs) var(--spacing-sm);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-sm);
    background-color: var(--bg-surface);
    color: var(--text-color);
    font-size: var(--font-size-sm);
    min-width: 0;
    flex: 1;
    max-width: 160px;
  }

  .config-row select:focus,
  .config-row input[type="text"]:focus {
    outline: none;
    border-color: var(--primary-color);
  }

  /* Full-width text input (e.g. File Name field) */
  .config-section > input[type="text"] {
    width: 100%;
    padding: var(--spacing-xs) var(--spacing-sm);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-sm);
    background-color: var(--bg-surface);
    color: var(--text-color);
    font-size: var(--font-size-sm);
    box-sizing: border-box;
  }

  .config-section > input[type="text"]:focus {
    outline: none;
    border-color: var(--primary-color);
  }

  /* Format hint text below selects */
  .format-hint {
    font-size: 0.75rem;
    color: var(--text-muted);
    padding-left: var(--spacing-xs);
    margin-top: -4px;
  }

  /* Trim validation */
  .input-error {
    border-color: var(--error-color) !important;
  }

  .trim-error {
    font-size: 0.75rem;
    color: var(--error-color);
    padding-left: var(--spacing-xs);
  }

  .trim-summary {
    font-size: 0.75rem;
    color: var(--success-color);
    padding-left: var(--spacing-xs);
  }

  .trim-hint {
    font-size: 0.75rem;
    color: var(--text-muted);
    padding-left: var(--spacing-xs);
  }

  .trim-slider-labels {
    display: flex;
    align-items: center;
    justify-content: space-between;
    font-size: 0.75rem;
    color: var(--text-color);
    font-variant-numeric: tabular-nums;
  }

  .trim-slider-labels .trim-total {
    color: var(--text-muted);
  }

  /* Download folder */
  .folder-row {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
  }

  .folder-path {
    flex: 1;
    font-size: var(--font-size-sm);
    color: var(--text-color);
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    min-width: 0;
  }

  .btn-browse {
    padding: var(--spacing-xs) var(--spacing-sm);
    font-size: var(--font-size-sm);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-sm);
    background-color: var(--bg-surface);
    color: var(--text-color);
    cursor: pointer;
    white-space: nowrap;
    flex-shrink: 0;
  }

  .btn-browse:hover {
    background-color: var(--bg-surface-hover);
  }

  /* Footer */
  .inspector-footer {
    display: flex;
    gap: var(--spacing-sm);
    padding: var(--spacing-md) var(--spacing-lg);
    border-top: 1px solid var(--border-color);
  }

  .inspector-footer .btn-primary {
    flex: 1;
    padding: var(--spacing-sm) var(--spacing-md);
    background-color: var(--primary-color);
    color: #fff;
    border: none;
    border-radius: var(--radius-md);
    cursor: pointer;
    font-size: var(--font-size-sm);
    font-weight: 600;
    transition: background-color 0.15s;
  }

  .inspector-footer .btn-primary:hover {
    background-color: var(--primary-hover);
  }

  .btn-secondary {
    padding: var(--spacing-sm) var(--spacing-md);
    background: none;
    border: 1px solid var(--border-color);
    border-radius: var(--radius-md);
    color: var(--text-muted);
    cursor: pointer;
    font-size: var(--font-size-sm);
    transition: background-color 0.15s, color 0.15s;
  }

  .btn-secondary:hover {
    background-color: var(--bg-surface-hover);
    color: var(--error-color);
    border-color: var(--error-color);
  }
</style>
