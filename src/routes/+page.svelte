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
    updateJobConfig,
    defaultConfig,
  } from '../stores/queue';
  import { downloadDefaults } from '../stores/downloadDefaults';
  import { get } from 'svelte/store';
  import { settings } from '../stores/settings';
  import { binaryCheckState, binaryStatus } from '../stores/binaries';
  import { addHistoryRecord, updateHistoryRecord, history } from '../stores/history';
  import { getCachedMetadata, cacheMetadata } from '../stores/metadataCache';
  import type { MediaJob, MediaMetadata, HistoryRecord, JobStatus } from '../types';
  import MediaCard from '../components/MediaCard.svelte';
  import InspectorPanel from '../components/InspectorPanel.svelte';
  import { buildCommandFromJob } from '../lib/ytdlpCommand';
  import { describeJobConfig } from '../lib/settingsSummary';
  import { isThumbnailEmbeddable } from '../lib/thumbnail';

  interface ProcessEvent {
    job_id: string;
    event_type: string;
    payload: string;
  }

  // Track resolved file paths emitted by yt-dlp (via --print after_move:filepath)
  const resolvedFilePaths = new Map<string, string>();

  // Track locally cached thumbnail paths
  const cachedThumbnailPaths = new Map<string, string>();

  // Job ids whose final file size has already been looked up.
  const sizeFetched = new Set<string>();

  /** Build a human-readable format label from job config */
  function buildFormatLabel(job: MediaJob): string {
    if (job.config.workflow === 'audio_only') {
      const fmt = (job.config.audioOnlyConfig?.format ?? 'mp3').toUpperCase();
      const q = job.config.audioOnlyConfig?.quality ?? 'balanced';
      const qLabel = q === 'best' ? 'Maximum Quality' : q === 'balanced' ? 'Recommended' : 'Compact';
      return `${fmt} - ${qLabel}`;
    }
    const fmt = (job.config.videoTranscode?.targetFormat ?? 'mp4').toUpperCase();
    const q = job.config.videoTranscode?.quality ?? 'balanced';
    const qLabel = q === 'best' ? 'Maximum Quality' : q === 'balanced' ? 'Recommended' : 'Compact';
    return `${fmt} - ${qLabel}`;
  }

  /**
   * Create or update the history record for a job at its current lifecycle
   * state. Invoked as soon as a job is added (queued/inspecting) and on every
   * status or metadata change, so in-progress and failed downloads are tracked
   * too — not just completed ones.
   */
  function syncHistoryRecord(job: MediaJob, statusOverride?: JobStatus): void {
    const existing = get(history).find((r) => r.id === job.id);
    const now = new Date().toISOString();
    const status = statusOverride ?? job.status;
    const terminal = status === 'completed' || status === 'error';
    const wasTerminal = existing ? existing.status === 'completed' || existing.status === 'error' : false;

    const record: HistoryRecord = {
      id: job.id,
      url: job.url,
      title: job.metadata?.title || existing?.title || job.url,
      uploader: job.metadata?.uploader ?? existing?.uploader ?? '',
      thumbnailUrl: job.metadata?.thumbnailUrl ?? existing?.thumbnailUrl ?? '',
      cachedThumbnailPath: cachedThumbnailPaths.get(job.id) ?? existing?.cachedThumbnailPath,
      durationSeconds: job.metadata?.durationSeconds ?? existing?.durationSeconds ?? 0,
      extractor: job.metadata?.extractor ?? existing?.extractor ?? '',
      filePath: resolvedFilePaths.get(job.id) ?? existing?.filePath ?? '',
      fileSize: existing?.fileSize,
      status,
      addedAt: existing?.addedAt ?? now,
      completedAt: terminal
        ? (wasTerminal && existing ? existing.completedAt : now)
        : (existing?.completedAt ?? now),
      errorMessage: status === 'error' ? job.errorMessage : undefined,
      workflow: job.config.workflow,
      formatLabel: buildFormatLabel(job),
    };

    if (existing) {
      void updateHistoryRecord(job.id, record);
    } else {
      void addHistoryRecord(record);
    }

    // Fill in the file size once the download has finished.
    if (status === 'completed' && record.filePath && record.fileSize == null && !sizeFetched.has(job.id)) {
      sizeFetched.add(job.id);
      invoke<number>('get_file_size', { path: record.filePath })
        .then((size) => updateHistoryRecord(job.id, { fileSize: size }))
        .catch(() => { /* non-critical */ });
    }
  }

  // Keep history in sync with the queue: whenever a tracked field changes
  // (status, metadata, error), upsert the corresponding history record. The
  // signature guard avoids rewriting history on every progress tick.
  let historySyncKey = '';
  $effect(() => {
    const key = $jobs
      .map((j) => `${j.id}|${j.status}|${j.metadata?.title ?? ''}|${j.metadata?.thumbnailUrl ?? ''}|${j.errorMessage ?? ''}`)
      .join(';');
    if (key === historySyncKey) return;
    historySyncKey = key;
    for (const job of $jobs) syncHistoryRecord(job);
  });

  // Comprehensive error message humanization
  const errorMappings: { match: string; message: string }[] = [
    { match: 'is not a valid URL', message: "This link doesn't appear to be a valid URL." },
    { match: 'HTTP Error 403', message: 'This content is restricted or unavailable in your region.' },
    { match: 'HTTP Error 404', message: 'This content was not found. It may have been removed.' },
    { match: 'Private video', message: 'This content is private and cannot be accessed.' },
    { match: 'Sign in', message: 'This content requires a login to access.' },
    { match: 'age-restricted', message: 'This content is age-restricted.' },
    { match: 'copyright', message: 'This content was removed due to a copyright claim.' },
    { match: 'No video formats', message: 'No downloadable media was found at this URL.' },
    { match: 'Unsupported URL', message: 'This website is not currently supported.' },
    { match: 'unavailable', message: 'This content is currently unavailable.' },
  ];

  function humanizeError(raw: string): string {
    for (const { match, message } of errorMappings) {
      if (raw.includes(match)) return message;
    }
    if (raw.length <= 400) return raw;
    // Keep the head for context and the tail, where yt-dlp/ffmpeg place the
    // actual failure (e.g. the offending output file path).
    return raw.substring(0, 120) + '\n…\n' + raw.substring(raw.length - 300);
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

        // Capture resolved file path from --print after_move:filepath
        // yt-dlp prints the absolute path as a plain line (e.g., C:\...\file.mp4 or /home/.../file.mp4)
        const trimmed = payload.trim();
        if (trimmed && !trimmed.includes('%') && !trimmed.startsWith('[') &&
            (trimmed.match(/^[A-Z]:\\/) || trimmed.startsWith('/'))) {
          resolvedFilePaths.set(jobId, trimmed);
        }
      } else if (event_type === 'exit') {
        updateJobProgress(jobId, { percentage: 100, currentStep: '' });
        updateJobStatus(jobId, 'completed');
        // Sync history synchronously, while the resolved output path is still
        // cached (it is cleared immediately below).
        const completedJob = $jobs.find(j => j.id === jobId);
        if (completedJob) {
          syncHistoryRecord({ ...completedJob, status: 'completed' });
        }
        resolvedFilePaths.delete(jobId);
        cachedThumbnailPaths.delete(jobId);
        cleanupListener(jobId);
      } else if (event_type === 'error') {
        const message = humanizeError(payload);
        updateJobStatus(jobId, 'error', message);
        // Record the failure in history so failed downloads are still tracked.
        const failedJob = $jobs.find(j => j.id === jobId);
        if (failedJob) {
          syncHistoryRecord({ ...failedJob, status: 'error', errorMessage: message });
        }
        cleanupListener(jobId);
      } else if (event_type === 'stderr') {
        // yt-dlp often writes progress to stderr; also capture errors.
        // Surface the error immediately but keep listening: the terminal
        // `error` event carries a richer stderr tail (verbose ffmpeg output),
        // and tearing the listener down here would drop it.
        if (payload.includes('ERROR')) {
          const raw = payload.replace(/^.*ERROR:\s*/, '');
          updateJobStatus(jobId, 'error', humanizeError(raw));
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
  // URLs from a multi-URL paste awaiting confirmation before queuing.
  let pendingUrls: string[] | null = $state(null);
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

  /** Read + clear the URL input, returning the trimmed value (or '' if empty). */
  function takeUrlInput(): string {
    if ($binaryCheckState !== 'done') return '';
    const url = urlInput.trim();
    if (!url) return '';
    urlInput = '';
    return url;
  }

  /**
   * Queue a single URL immediately using the remembered download defaults
   * (or a fresh default config on first run), fetching metadata in the
   * background so the card/history fill in without blocking the download.
   */
  function queueUrlWithDefaults(url: string) {
    const cfg = get(downloadDefaults) ?? defaultConfig();
    // Add as 'inspecting' so the download waits for metadata: the selected
    // thumbnail's format decides whether yt-dlp can safely embed it (see
    // isThumbnailEmbeddable).
    const id = addJob(url);
    updateJobConfig(id, cfg);

    const cached = getCachedMetadata(url);
    if (cached) {
      updateJobMetadata(id, cached);
      updateJobStatus(id, 'queued');
      return;
    }

    invoke('fetch_metadata', { url })
      .then((metadata) => {
        updateJobMetadata(id, metadata as MediaMetadata);
        cacheMetadata(url, metadata as MediaMetadata);
        const thumb = (metadata as MediaMetadata).thumbnailUrl;
        if (thumb) {
          invoke('cache_thumbnail', { url: thumb, jobId: id }).then((localPath) => {
            cachedThumbnailPaths.set(id, localPath as string);
          }).catch(() => { /* non-critical */ });
        }
      })
      .catch(() => { /* non-critical; download still proceeds */ })
      .finally(() => updateJobStatus(id, 'queued'));
  }

  /** Default Add: queue the current input with the remembered defaults. */
  function handleQuickAdd() {
    const url = takeUrlInput();
    if (!url) return;
    queueUrlWithDefaults(url);
  }

  /**
   * Detect a pasted list of URLs. The text is stripped of surrounding
   * whitespace and split on any run of whitespace (spaces, tabs, newlines);
   * if every resulting token is a valid http(s) URL and there is more than
   * one, the individual URLs are returned. Otherwise null, so ordinary text
   * pastes fall through untouched.
   */
  function parseUrlList(text: string): string[] | null {
    const tokens = text.trim().split(/\s+/).filter(Boolean);
    if (tokens.length < 2) return null;
    if (!tokens.every((token) => isValidUrl(token))) return null;
    return tokens;
  }

  function handleInputPaste(e: ClipboardEvent) {
    const text = e.clipboardData?.getData('text') ?? '';
    if (!text) return;
    const urls = parseUrlList(text);
    if (urls) {
      // Take over the paste: confirm before queuing the whole list.
      e.preventDefault();
      pendingUrls = urls;
    }
  }

  /**
   * Add w/ Options: inspect the URL, then open the configure pane so the user
   * can adjust settings before queueing.
   */
  async function handleAddWithOptions() {
    const url = takeUrlInput();
    if (!url) return;

    const jobId = addJob(url);
    // Auto-select the new job
    selectedJobId.set(jobId);

    // Check metadata cache first for instant UI population
    const cached = getCachedMetadata(url);
    if (cached) {
      updateJobMetadata(jobId, cached);
      updateJobStatus(jobId, 'configuring');
      return;
    }

    try {
      const metadata: MediaMetadata = await invoke('fetch_metadata', { url });
      updateJobMetadata(jobId, metadata);
      updateJobStatus(jobId, 'configuring');
      // Cache metadata for future use
      cacheMetadata(url, metadata);
      // Cache thumbnail in background for history
      if (metadata.thumbnailUrl) {
        invoke('cache_thumbnail', { url: metadata.thumbnailUrl, jobId }).then((localPath) => {
          cachedThumbnailPaths.set(jobId, localPath as string);
        }).catch(() => { /* non-critical */ });
      }
    } catch (err) {
      updateJobStatus(jobId, 'error', err instanceof Error ? err.message : String(err));
    }
  }

  function handleInputKeydown(e: KeyboardEvent) {
    if (e.key === 'Enter') {
      // Shift+Enter opens the options pane; Enter uses the remembered defaults.
      if (e.shiftKey) {
        handleAddWithOptions();
      } else {
        handleQuickAdd();
      }
    }
  }

  /** Confirm a multi-URL paste: queue every URL with the default Add options. */
  function confirmBulkAdd() {
    if (!pendingUrls || $binaryCheckState !== 'done') return;
    const urls = pendingUrls;
    pendingUrls = null;
    urlInput = '';
    for (const url of urls) queueUrlWithDefaults(url);
  }

  function cancelBulkAdd() {
    pendingUrls = null;
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
      handleQuickAdd();
    }

    // Also check for text/uri-list
    const uriList = e.dataTransfer?.getData('text/uri-list') ?? '';
    if (!text && uriList) {
      const firstUrl = uriList.split('\n').find((line) => line.trim() && !line.startsWith('#'));
      if (firstUrl) {
        urlInput = firstUrl.trim();
        handleQuickAdd();
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
    if (pendingUrls && e.key === 'Escape') {
      e.preventDefault();
      cancelBulkAdd();
      return;
    }

    // Don't handle if already in an input
    const tag = (e.target as HTMLElement)?.tagName;
    if (tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT') return;

    if ((e.ctrlKey || e.metaKey) && e.key === 'v') {
      e.preventDefault();
      urlInputEl?.focus();
      // Read clipboard and populate
      navigator.clipboard.readText().then((text) => {
        const urls = parseUrlList(text);
        if (urls) {
          pendingUrls = urls;
          return;
        }
        const trimmed = text.trim();
        if (trimmed && isValidUrl(trimmed)) {
          urlInput = trimmed;
          handleQuickAdd();
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

  // Settings the default "Add" button will use (the remembered defaults, or a
  // fresh default config on first run). Shown beneath the URL input.
  let quickAddSummary = $derived(
    describeJobConfig($downloadDefaults ?? defaultConfig())
      .map((row) => `${row.label}: ${row.value}`)
      .join(' | ')
  );

  // Context menu
  let contextMenu = $state<{ x: number; y: number; cmd: string; job: MediaJob } | null>(null);
  let copiedCmd = $state(false);
  let copyTimeout: ReturnType<typeof setTimeout> | null = null;

  function openContextMenu(e: MouseEvent, job: MediaJob) {
    e.preventDefault();
    const cmd = buildCommandFromJob(
      job,
      $settings.downloadPath,
      $settings.useImpersonateChrome,
      $settings.useNoCookies,
      $binaryStatus?.yt_dlp_path,
      $binaryStatus?.ffmpeg_path,
    );
    contextMenu = { x: e.clientX, y: e.clientY, cmd, job };
    copiedCmd = false;
  }

  function closeContextMenu() {
    contextMenu = null;
  }

  async function copyYtDlpCommand() {
    if (!contextMenu) return;
    await navigator.clipboard.writeText(contextMenu.cmd);
    contextMenu = null;
    copiedCmd = true;
    if (copyTimeout) clearTimeout(copyTimeout);
    copyTimeout = setTimeout(() => { copiedCmd = false; }, 1500);
  }

  // Queue search/filter
  let queueSearch = $state('');

  let filteredQueueJobs = $derived.by(() => {
    let result = nonCompletedJobs;
    if (queueSearch.trim()) {
      const q = queueSearch.toLowerCase();
      result = result.filter(j =>
        (j.metadata?.title ?? '').toLowerCase().includes(q) ||
        j.url.toLowerCase().includes(q) ||
        (j.metadata?.uploader ?? '').toLowerCase().includes(q)
      );
    }
    return result;
  });

  // Watch for queued jobs and start downloads (respecting concurrent limit)
  let prevStartedIds = new Set<string>();
  $effect(() => {
    const activeCount = $jobs.filter(
      j => j.status === 'downloading' || j.status === 'processing'
    ).length;
    const queued = $jobs.filter(j => j.status === 'queued');
    const slots = $settings.concurrentDownloads - activeCount;

    if (slots > 0 && queued.length > 0 && !$settings.globalPaused) {
      for (const job of queued.slice(0, slots)) {
        if (!prevStartedIds.has(job.id)) {
          prevStartedIds.add(job.id);
          startJobDownload(job);
        }
      }
    }
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
          outputPath: job.config.downloadPath || $settings.downloadPath,
          outputFilename: job.config.outputFilename,
          targetFormat: job.config.videoTranscode?.targetFormat,
          videoQuality: job.config.videoTranscode?.quality,
          audioFormat: job.config.audioOnlyConfig?.format,
          audioQuality: job.config.audioOnlyConfig?.quality,
          embedSubtitles: job.config.embedSubtitles,
          embedMetadata: job.config.embedMetadata,
          // yt-dlp can't embed thumbnails it has to convert with ffmpeg's
          // image2 demuxer (e.g. AVIF) — skip embedding rather than fail.
          embedThumbnail: job.config.embedThumbnail && isThumbnailEmbeddable(job.metadata?.thumbnailUrl),
          trimStart: job.config.trim?.start,
          trimEnd: job.config.trim?.end,
          useImpersonateChrome: $settings.useImpersonateChrome,
          useNoCookies: $settings.useNoCookies,
          collectLogs: $settings.collectLogs,
        },
      });
    } catch (err) {
      const message = err instanceof Error ? err.message : String(err);
      updateJobStatus(job.id, 'error', message);
      cleanupListener(job.id);
    }
  }
</script>

<svelte:window onkeydown={handleGlobalKeydown} onclick={closeContextMenu} />

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
          onpaste={handleInputPaste}
          disabled={$binaryCheckState !== 'done'}
        />
        <button
          class="btn-primary"
          onclick={handleQuickAdd}
          disabled={!urlInput.trim() || $binaryCheckState !== 'done'}
          title="Download now with your last-used settings (Enter)"
        >
          Add
        </button>
        <button
          class="btn-options"
          onclick={handleAddWithOptions}
          disabled={!urlInput.trim() || $binaryCheckState !== 'done'}
          title="Choose settings before downloading (Shift+Enter)"
        >
          Add w/ Options
        </button>
      </div>

      {#if dragOver}
        <div class="drop-overlay">Drop URL here</div>
      {/if}

      <!-- Default settings + disk space -->
      <div class="input-meta">
        <span class="defaults-summary">{quickAddSummary}</span>
        <div class="input-meta-right">
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
    </div>

    {#if $settings.globalPaused}
      <div class="pause-banner">Queue is paused. Press Space to resume.</div>
    {/if}

    <!-- Queue Header -->
    <header class="view-header">
      <h2>Queue</h2>
      <span class="count">{$activeJobs.length} active</span>
    </header>

    {#if nonCompletedJobs.length > 3}
      <div class="queue-search">
        <input
          type="text"
          class="search-input"
          placeholder="Filter queue..."
          bind:value={queueSearch}
        />
      </div>
    {/if}

    {#if $jobs.length === 0}
      <div class="empty-state">
        <p>No jobs in the queue.</p>
        <p class="hint">Paste a URL above or drag a link to get started.</p>
      </div>
    {:else}
      <ul class="job-list">
        {#each filteredQueueJobs as job (job.id)}
          <li
            class="job-list-item"
            class:item-selected={$selectedJobId === job.id}
            class:item-error={job.status === 'error'}
            oncontextmenu={(e) => openContextMenu(e, job)}
          >
            <div class="job-card-wrap">
              <MediaCard
                {job}
                selected={$selectedJobId === job.id}
                onselect={() => selectJob(job.id)}
              />
            </div>
            <div class="job-actions">
              {#if job.status === 'error'}
                <button
                  class="btn-sm"
                  onclick={() => {
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

{#if contextMenu}
  <!-- svelte-ignore a11y_no_noninteractive_element_interactions -->
  <ul
    class="context-menu"
    style="left: {contextMenu.x}px; top: {contextMenu.y}px"
    onclick={(e) => e.stopPropagation()}
    onkeydown={(e) => e.key === 'Escape' && closeContextMenu()}
    role="menu"
  >
    <li class="context-menu-item" role="menuitem" onclick={copyYtDlpCommand}>
      Copy yt-dlp command
    </li>
    <li class="context-menu-item" role="menuitem" onclick={() => { navigator.clipboard.writeText(contextMenu!.job.url); closeContextMenu(); }}>
      Copy source URL
    </li>
    {#if contextMenu.job.errorMessage}
      <li class="context-menu-item" role="menuitem" onclick={() => { navigator.clipboard.writeText(contextMenu!.job.errorMessage!); closeContextMenu(); }}>
        Copy error to clipboard
      </li>
    {/if}
  </ul>
{/if}

{#if pendingUrls}
  <div class="bulk-overlay" role="presentation" onclick={cancelBulkAdd}>
    <div
      class="bulk-dialog"
      role="dialog"
      aria-modal="true"
      aria-labelledby="bulk-dialog-title"
      tabindex="-1"
      onclick={(e) => e.stopPropagation()}
      onkeydown={(e) => e.key === 'Escape' && cancelBulkAdd()}
    >
      <h3 id="bulk-dialog-title">Add {pendingUrls.length} URLs?</h3>
      <p class="bulk-desc">
        These links will be queued with your default settings: {quickAddSummary}
      </p>
      <ul class="bulk-list">
        {#each pendingUrls as url}
          <li title={url}>{url}</li>
        {/each}
      </ul>
      <div class="bulk-actions">
        <button class="btn-secondary" onclick={cancelBulkAdd}>Cancel</button>
        <button class="btn-primary" onclick={confirmBulkAdd}>Add All ({pendingUrls.length})</button>
      </div>
    </div>
  </div>
{/if}

{#if copiedCmd}
  <div class="copy-toast">Command copied!</div>
{/if}

<style>
  .queue-layout {
    display: flex;
    height: 100%;
    overflow: hidden;
  }

  .view {
    flex: 1;
    min-width: 0;
    max-width: 900px;
    overflow-y: auto;
    padding: var(--spacing-lg) var(--spacing-xl);
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

  .queue-search {
    margin-bottom: var(--spacing-sm);
  }

  .search-input {
    width: 100%;
    padding: var(--spacing-xs) var(--spacing-md);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-md);
    background-color: var(--bg-surface);
    color: var(--text-color);
    font-size: 0.85rem;
    transition: border-color 0.15s ease;
  }

  .search-input:focus {
    outline: none;
    border-color: var(--primary-color);
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

  .btn-options {
    padding: var(--spacing-sm) var(--spacing-md);
    background-color: var(--bg-surface);
    color: var(--text-muted);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-md);
    cursor: pointer;
    font-size: 0.9rem;
    font-weight: 600;
    white-space: nowrap;
    transition: background-color 0.15s, color 0.15s, border-color 0.15s;
  }

  .btn-options:hover:not(:disabled) {
    background-color: var(--bg-surface-hover);
    color: var(--text-color);
  }

  .btn-options:disabled {
    opacity: 0.4;
    cursor: not-allowed;
  }

  .input-meta {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: var(--spacing-sm);
    margin-top: var(--spacing-xs);
    padding: 0 var(--spacing-xs);
  }

  .defaults-summary {
    font-size: 0.75rem;
    color: var(--text-muted);
    min-width: 0;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .input-meta-right {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
    flex-shrink: 0;
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
    gap: var(--spacing-md);
  }

  /* Each item IS the card — buttons live inside it */
  .job-list-item {
    display: flex;
    flex-direction: column;
    background: var(--card-bg);
    border: 2px solid transparent;
    border-radius: var(--radius-lg);
    overflow: hidden;
    transition: border-color 0.15s ease, box-shadow 0.15s ease;
  }

  .job-list-item:hover {
    border-color: var(--border-color);
  }

  .job-list-item.item-selected {
    border-color: var(--primary-color);
    box-shadow: 0 0 0 1px var(--primary-color);
  }

  .job-list-item.item-error {
    border-color: var(--error-color);
  }

  .job-card-wrap {
    flex: 1;
    min-width: 0;
  }

  /* Strip MediaCard's own card chrome — the parent li handles it */
  .job-card-wrap :global(.media-card) {
    border: none !important;
    background: transparent !important;
    border-radius: 0 !important;
    box-shadow: none !important;
  }

  .job-card-wrap :global(.media-card:hover),
  .job-card-wrap :global(.media-card.selected),
  .job-card-wrap :global(.media-card.error) {
    border-color: transparent !important;
    box-shadow: none !important;
  }

  .job-actions {
    display: flex;
    flex-direction: row;
    justify-content: flex-end;
    align-items: center;
    gap: var(--spacing-sm);
    padding: var(--spacing-xs) var(--spacing-md) var(--spacing-sm);
    border-top: 1px solid var(--border-color);
  }

  .btn-sm {
    padding: var(--spacing-xs) var(--spacing-md);
    font-size: 0.8rem;
    border: 1px solid var(--border-color);
    border-radius: var(--radius-sm);
    background-color: var(--bg-surface);
    color: var(--text-color);
    cursor: pointer;
    transition: background-color 0.15s;
    white-space: nowrap;
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

  .context-menu {
    position: fixed;
    z-index: 1000;
    list-style: none;
    background-color: var(--bg-surface);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-md);
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.2);
    padding: var(--spacing-xs) 0;
    min-width: 180px;
  }

  .context-menu-item {
    padding: var(--spacing-sm) var(--spacing-md);
    font-size: 0.875rem;
    color: var(--text-color);
    cursor: pointer;
    user-select: none;
  }

  .context-menu-item:hover {
    background-color: var(--bg-surface-hover);
  }

  .copy-toast {
    position: fixed;
    bottom: var(--spacing-lg);
    left: 50%;
    transform: translateX(-50%);
    background-color: var(--bg-surface);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-md);
    padding: var(--spacing-xs) var(--spacing-md);
    font-size: 0.875rem;
    color: var(--success-color);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
    pointer-events: none;
    z-index: 1001;
  }

  /* Multi-URL paste confirmation dialog */
  .bulk-overlay {
    position: fixed;
    inset: 0;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 2000;
    padding: var(--spacing-lg);
  }

  .bulk-dialog {
    background-color: var(--bg-surface);
    color: var(--text-color);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-lg);
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.35);
    padding: var(--spacing-lg);
    width: 100%;
    max-width: 480px;
    display: flex;
    flex-direction: column;
    gap: var(--spacing-md);
  }

  .bulk-dialog h3 {
    font-size: 1.1rem;
    font-weight: 700;
  }

  .bulk-desc {
    font-size: var(--font-size-sm);
    color: var(--text-muted);
    line-height: 1.4;
  }

  .bulk-list {
    list-style: none;
    max-height: 220px;
    overflow-y: auto;
    border: 1px solid var(--border-color);
    border-radius: var(--radius-md);
    background-color: var(--bg-color);
    padding: var(--spacing-sm);
    display: flex;
    flex-direction: column;
    gap: var(--spacing-xs);
  }

  .bulk-list li {
    font-size: 0.8rem;
    color: var(--text-color);
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    font-variant-numeric: tabular-nums;
  }

  .bulk-actions {
    display: flex;
    justify-content: flex-end;
    gap: var(--spacing-sm);
  }

  .bulk-actions .btn-secondary {
    padding: var(--spacing-sm) var(--spacing-md);
    background: none;
    border: 1px solid var(--border-color);
    border-radius: var(--radius-md);
    color: var(--text-muted);
    cursor: pointer;
    font-size: 0.9rem;
    font-weight: 600;
    transition: background-color 0.15s, color 0.15s;
  }

  .bulk-actions .btn-secondary:hover {
    background-color: var(--bg-surface-hover);
    color: var(--text-color);
  }
</style>
