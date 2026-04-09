<script lang="ts">
  import { invoke } from '@tauri-apps/api/core';
  import { listen } from '@tauri-apps/api/event';
  import {
    jobs,
    activeJobs,
    selectedJobId,
    addJob,
    removeJob,
    updateJobStatus,
    updateJobMetadata,
    updateJobProgress,
  } from '../stores/queue';
  import { settings } from '../stores/settings';
  import { binaryCheckState } from '../stores/binaries';
  import type { MediaJob, MediaMetadata } from '../types';
  import MediaCard from '../components/MediaCard.svelte';
  import InspectorPanel from '../components/InspectorPanel.svelte';

  interface ProcessEvent {
    job_id: string;
    event_type: string;
    payload: string;
  }

  // Map of active process event listeners (jobId -> unlisten fn)
  const processListeners = new Map<string, () => void>();

  /** Start listening for process events for a given job */
  async function listenToProcess(jobId: string) {
    // Clean up any existing listener
    processListeners.get(jobId)?.();

    const unlisten = await listen<ProcessEvent>(`process-event-${jobId}`, (event) => {
      const { event_type, payload } = event.payload;

      if (event_type === 'stdout') {
        // Parse yt-dlp progress output
        const percentMatch = payload.match(/(\d+(?:\.\d+)?)%/);
        const speedMatch = payload.match(/(\d+(?:\.\d+)?\s*\S+\/s)/);
        const etaMatch = payload.match(/ETA\s+(\S+)/);

        if (percentMatch) {
          updateJobProgress(jobId, {
            percentage: parseFloat(percentMatch[1]),
            ...(speedMatch ? { downloadSpeed: speedMatch[1] } : {}),
            ...(etaMatch ? { eta: etaMatch[1] } : {}),
          });
        }

        // Detect merging/processing step
        if (payload.includes('[Merger]') || payload.includes('[ffmpeg]')) {
          updateJobProgress(jobId, { currentStep: 'Merging streams...' });
          updateJobStatus(jobId, 'processing');
        }
      } else if (event_type === 'exit') {
        updateJobProgress(jobId, { percentage: 100, currentStep: '' });
        updateJobStatus(jobId, 'completed');
        cleanupListener(jobId);
      } else if (event_type === 'error') {
        // User-friendly error message
        let message = payload;
        if (message.includes('is not a valid URL')) {
          message = 'The provided URL is not valid or supported.';
        } else if (message.includes('HTTP Error 403')) {
          message = 'Video is unavailable or geo-restricted (HTTP 403).';
        } else if (message.includes('HTTP Error 404')) {
          message = 'Video not found (HTTP 404). It may have been removed.';
        } else if (message.includes('Private video')) {
          message = 'This video is private and cannot be downloaded.';
        } else if (message.length > 200) {
          message = message.substring(0, 200) + '...';
        }
        updateJobStatus(jobId, 'error', message);
        cleanupListener(jobId);
      } else if (event_type === 'stderr') {
        // yt-dlp often writes progress to stderr; also capture errors
        if (payload.includes('ERROR')) {
          let message = payload.replace(/^.*ERROR:\s*/, '');
          if (message.length > 200) {
            message = message.substring(0, 200) + '...';
          }
          updateJobStatus(jobId, 'error', message);
          cleanupListener(jobId);
        }
      }
    });

    processListeners.set(jobId, unlisten);
  }

  function cleanupListener(jobId: string) {
    processListeners.get(jobId)?.();
    processListeners.delete(jobId);
  }

  let urlInput = $state('');
  let urlInputEl: HTMLInputElement | undefined = $state(undefined);
  let diskAvailable = $state<number | null>(null);
  let diskTotal = $state<number | null>(null);
  let dragOver = $state(false);

  // Fetch disk space on mount and when download path changes
  $effect(() => {
    const path = $settings.downloadPath;
    fetchDiskSpace(path);
  });

  async function fetchDiskSpace(path: string) {
    try {
      const info: { availableBytes: number; totalBytes: number } = await invoke('get_disk_space', { path });
      diskAvailable = info.availableBytes;
      diskTotal = info.totalBytes;
    } catch {
      diskAvailable = null;
      diskTotal = null;
    }
  }

  function formatBytes(bytes: number): string {
    if (bytes < 1024 * 1024 * 1024) return `${(bytes / (1024 * 1024)).toFixed(0)} MB`;
    return `${(bytes / (1024 * 1024 * 1024)).toFixed(1)} GB`;
  }

  /** Returns true if any inspecting job has a large estimated size */
  function hasLargeSizeWarning(): boolean {
    if (diskAvailable === null) return false;
    // Warn if available space is below 2 GB
    return diskAvailable < 2 * 1024 * 1024 * 1024;
  }

  // -------------------------------------------------------------------------
  // Ingestion flow
  // -------------------------------------------------------------------------

  async function handleAddUrl() {
    if ($binaryCheckState !== 'done') return;
    const url = urlInput.trim();
    if (!url) return;
    urlInput = '';

    const jobId = addJob(url);
    // Auto-select the new job
    selectedJobId.set(jobId);

    try {
      const metadata: MediaMetadata = await invoke('fetch_metadata', { url });
      updateJobMetadata(jobId, metadata);
      updateJobStatus(jobId, 'configuring');
    } catch (err) {
      updateJobStatus(jobId, 'error', err instanceof Error ? err.message : String(err));
    }
  }

  function handleInputKeydown(e: KeyboardEvent) {
    if (e.key === 'Enter') {
      handleAddUrl();
    }
  }

  // -------------------------------------------------------------------------
  // Drag and drop
  // -------------------------------------------------------------------------

  function handleDragOver(e: DragEvent) {
    e.preventDefault();
    dragOver = true;
  }

  function handleDragLeave() {
    dragOver = false;
  }

  function handleDrop(e: DragEvent) {
    e.preventDefault();
    dragOver = false;

    const text = e.dataTransfer?.getData('text/plain') ?? '';
    if (text && isValidUrl(text)) {
      urlInput = text;
      handleAddUrl();
    }

    // Also check for text/uri-list
    const uriList = e.dataTransfer?.getData('text/uri-list') ?? '';
    if (!text && uriList) {
      const firstUrl = uriList.split('\n').find((line) => line.trim() && !line.startsWith('#'));
      if (firstUrl) {
        urlInput = firstUrl.trim();
        handleAddUrl();
      }
    }
  }

  function isValidUrl(str: string): boolean {
    try {
      const url = new URL(str);
      return url.protocol === 'http:' || url.protocol === 'https:';
    } catch {
      return false;
    }
  }

  // -------------------------------------------------------------------------
  // Keyboard shortcut: Ctrl/Cmd + V to focus input
  // -------------------------------------------------------------------------

  function handleGlobalKeydown(e: KeyboardEvent) {
    // Don't handle if already in an input
    const tag = (e.target as HTMLElement)?.tagName;
    if (tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT') return;

    if ((e.ctrlKey || e.metaKey) && e.key === 'v') {
      e.preventDefault();
      urlInputEl?.focus();
      // Read clipboard and populate
      navigator.clipboard.readText().then((text) => {
        const trimmed = text.trim();
        if (trimmed && isValidUrl(trimmed)) {
          urlInput = trimmed;
          handleAddUrl();
        } else if (trimmed) {
          urlInput = trimmed;
        }
      }).catch(() => {
        // Clipboard not available, just focus
      });
    }
  }

  // -------------------------------------------------------------------------
  // Job selection / Inspector
  // -------------------------------------------------------------------------

  function selectJob(id: string) {
    selectedJobId.update((current) => (current === id ? null : id));
  }

  function closeInspector() {
    selectedJobId.set(null);
  }

  let inspectorJob = $derived(
    $selectedJobId
      ? $jobs.find((j) => j.id === $selectedJobId && j.status === 'configuring') ?? null
      : null
  );

  let nonCompletedJobs = $derived($jobs.filter((j) => j.status !== 'completed'));

  // Watch for queued jobs and start downloads
  let prevQueuedIds = new Set<string>();
  $effect(() => {
    const queued = $jobs.filter((j) => j.status === 'queued');
    for (const job of queued) {
      if (!prevQueuedIds.has(job.id)) {
        startJobDownload(job);
      }
    }
    prevQueuedIds = new Set(queued.map((j) => j.id));
  });

  async function startJobDownload(job: MediaJob) {
    updateJobStatus(job.id, 'downloading');
    updateJobProgress(job.id, { percentage: 0, currentStep: 'Starting download...' });

    await listenToProcess(job.id);

    try {
      await invoke('start_download', {
        config: {
          jobId: job.id,
          url: job.url,
          workflow: job.config.workflow,
          outputPath: $settings.downloadPath,
          targetFormat: job.config.videoTranscode?.targetFormat,
          videoQuality: job.config.videoTranscode?.quality,
          audioFormat: job.config.audioOnlyConfig?.format,
          audioQuality: job.config.audioOnlyConfig?.quality,
          embedSubtitles: job.config.embedSubtitles,
          embedMetadata: job.config.embedMetadata,
          embedThumbnail: job.config.embedThumbnail,
          trimStart: job.config.trim?.start,
          trimEnd: job.config.trim?.end,
        },
      });
    } catch (err) {
      const message = err instanceof Error ? err.message : String(err);
      updateJobStatus(job.id, 'error', message);
      cleanupListener(job.id);
    }
  }
</script>

<svelte:window onkeydown={handleGlobalKeydown} />

<div class="queue-layout">
  <div class="view">
    <!-- Input Zone -->
    <div
      class="input-zone"
      class:drag-over={dragOver}
      ondragover={handleDragOver}
      ondragleave={handleDragLeave}
      ondrop={handleDrop}
      role="region"
      aria-label="URL input zone"
    >
      <div class="input-row">
        <input
          bind:this={urlInputEl}
          type="text"
          class="url-input"
          placeholder="Paste a URL to download... (Ctrl+V)"
          bind:value={urlInput}
          onkeydown={handleInputKeydown}
          disabled={$binaryCheckState !== 'done'}
        />
        <button class="btn-primary" onclick={handleAddUrl} disabled={!urlInput.trim() || $binaryCheckState !== 'done'}>
          Add
        </button>
      </div>

      {#if dragOver}
        <div class="drop-overlay">Drop URL here</div>
      {/if}

      <!-- Disk space indicator -->
      <div class="input-meta">
        {#if diskAvailable !== null}
          <span class="disk-info" class:disk-warning={hasLargeSizeWarning()}>
            {formatBytes(diskAvailable)} free
            {#if diskTotal !== null}
              of {formatBytes(diskTotal)}
            {/if}
          </span>
        {/if}
        {#if hasLargeSizeWarning()}
          <span class="disk-warning-text">Low disk space!</span>
        {/if}
      </div>
    </div>

    {#if $settings.globalPaused}
      <div class="pause-banner">Queue is paused. Press Space to resume.</div>
    {/if}

    <!-- Queue Header -->
    <header class="view-header">
      <h2>Queue</h2>
      <span class="count">{$activeJobs.length} active</span>
    </header>

    {#if $jobs.length === 0}
      <div class="empty-state">
        <p>No jobs in the queue.</p>
        <p class="hint">Paste a URL above or drag a link to get started.</p>
      </div>
    {:else}
      <ul class="job-list">
        {#each nonCompletedJobs as job (job.id)}
          <li class="job-list-item">
            <MediaCard
              {job}
              selected={$selectedJobId === job.id}
              onselect={() => selectJob(job.id)}
            />
            <div class="job-actions">
              {#if job.status === 'error'}
                <button
                  class="btn-sm"
                  onclick={() => {
                    // Re-run ingestion for errored jobs
                    updateJobStatus(job.id, 'inspecting');
                    invoke('fetch_metadata', { url: job.url })
                      .then((metadata) => {
                        updateJobMetadata(job.id, metadata as MediaMetadata);
                        updateJobStatus(job.id, 'configuring');
                      })
                      .catch((err) => {
                        updateJobStatus(job.id, 'error', err instanceof Error ? err.message : String(err));
                      });
                  }}
                >
                  Retry
                </button>
              {/if}
              <button class="btn-sm btn-danger" onclick={() => removeJob(job.id)}>
                Remove
              </button>
            </div>
          </li>
        {/each}
      </ul>
    {/if}
  </div>

  <!-- Inspector Panel (slides in for configuring jobs) -->
  {#if inspectorJob}
    <InspectorPanel job={inspectorJob} onclose={closeInspector} />
  {/if}
</div>

<style>
  .queue-layout {
    display: flex;
    height: 100%;
    gap: 0;
  }

  .view {
    flex: 1;
    min-width: 0;
    max-width: 900px;
  }

  /* Input zone */
  .input-zone {
    position: relative;
    margin-bottom: var(--spacing-lg);
  }

  .input-row {
    display: flex;
    gap: var(--spacing-sm);
  }

  .input-zone.drag-over {
    outline: 2px dashed var(--primary-color);
    outline-offset: 4px;
    border-radius: var(--radius-md);
  }

  .drop-overlay {
    position: absolute;
    inset: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: rgba(0, 123, 255, 0.1);
    border-radius: var(--radius-md);
    color: var(--primary-color);
    font-weight: 600;
    font-size: 1rem;
    pointer-events: none;
  }

  .url-input {
    flex: 1;
    padding: var(--spacing-sm) var(--spacing-md);
    border: 2px solid var(--border-color);
    border-radius: var(--radius-md);
    background-color: var(--bg-surface);
    color: var(--text-color);
    font-size: 1rem;
    transition: border-color 0.15s ease;
  }

  .url-input:focus {
    outline: none;
    border-color: var(--primary-color);
  }

  .btn-primary {
    padding: var(--spacing-sm) var(--spacing-lg);
    background-color: var(--primary-color);
    color: #fff;
    border: none;
    border-radius: var(--radius-md);
    cursor: pointer;
    font-size: 1rem;
    font-weight: 600;
  }

  .btn-primary:hover:not(:disabled) {
    background-color: var(--primary-hover);
  }

  .btn-primary:disabled {
    opacity: 0.4;
    cursor: not-allowed;
  }

  .input-meta {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
    margin-top: var(--spacing-xs);
    padding: 0 var(--spacing-xs);
  }

  .disk-info {
    font-size: 0.75rem;
    color: var(--text-muted);
  }

  .disk-info.disk-warning {
    color: var(--warning-color);
  }

  .disk-warning-text {
    font-size: 0.75rem;
    color: var(--warning-color);
    font-weight: 600;
  }

  /* Pause banner */
  .pause-banner {
    background-color: rgba(255, 193, 7, 0.15);
    color: var(--warning-color);
    padding: var(--spacing-sm) var(--spacing-md);
    border-radius: var(--radius-md);
    margin-bottom: var(--spacing-md);
    font-weight: 500;
    text-align: center;
  }

  /* Queue header */
  .view-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: var(--spacing-md);
  }

  .view-header h2 {
    font-size: 1.5rem;
    font-weight: 700;
  }

  .count {
    font-size: var(--font-size-sm);
    color: var(--text-muted);
  }

  .empty-state {
    text-align: center;
    padding: var(--spacing-xl) 0;
    color: var(--text-muted);
  }

  .hint {
    font-size: var(--font-size-sm);
    opacity: 0.7;
    margin-top: var(--spacing-xs);
  }

  /* Job list */
  .job-list {
    list-style: none;
    display: flex;
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .job-list-item {
    position: relative;
  }

  .job-actions {
    position: absolute;
    top: var(--spacing-md);
    right: var(--spacing-md);
    display: flex;
    gap: var(--spacing-xs);
  }

  .btn-sm {
    padding: 2px var(--spacing-sm);
    font-size: 0.8rem;
    border: 1px solid var(--border-color);
    border-radius: var(--radius-sm);
    background-color: var(--bg-surface);
    color: var(--text-color);
    cursor: pointer;
  }

  .btn-sm:hover {
    background-color: var(--bg-surface-hover);
  }

  .btn-danger {
    color: var(--error-color);
    border-color: var(--error-color);
  }

  .btn-danger:hover {
    background-color: rgba(220, 53, 69, 0.1);
  }
</style>
