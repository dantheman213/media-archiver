<script lang="ts">
  import type { MediaJob } from '../types';

  let { job, selected = false, onselect }: {
    job: MediaJob;
    selected?: boolean;
    onselect?: () => void;
  } = $props();

  const statusLabels: Record<string, string> = {
    inspecting: 'Inspecting...',
    configuring: 'Ready to configure',
    queued: 'Queued',
    paused: 'Paused',
    downloading: 'Downloading',
    processing: 'Processing',
    completed: 'Completed',
    error: 'Error',
  };

  function formatDuration(seconds: number): string {
    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const s = Math.floor(seconds % 60);
    if (h > 0) return `${h}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
    return `${m}:${String(s).padStart(2, '0')}`;
  }

  function formatFilesize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    if (bytes < 1024 * 1024 * 1024) return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
    return `${(bytes / (1024 * 1024 * 1024)).toFixed(2)} GB`;
  }

  /** Estimate total size from the best available format */
  function estimateSize(job: MediaJob): string | null {
    if (!job.metadata) return null;
    const sizes = job.metadata.availableFormats
      .map((f) => f.filesize ?? f.filesizeApprox)
      .filter((s): s is number => s != null && s > 0);
    if (sizes.length === 0) return null;
    return formatFilesize(Math.max(...sizes));
  }
</script>

<div
  class="media-card"
  class:selected
  class:inspecting={job.status === 'inspecting'}
  class:error={job.status === 'error'}
  role="button"
  tabindex="0"
  onclick={onselect}
  onkeydown={(e) => { if (e.key === 'Enter' && onselect) onselect(); }}
>
  {#if job.status === 'inspecting'}
    <!-- Skeleton loading state -->
    <div class="card-skeleton">
      <div class="skeleton-thumb pulse"></div>
      <div class="skeleton-info">
        <div class="skeleton-line wide pulse"></div>
        <div class="skeleton-line medium pulse"></div>
        <div class="skeleton-line narrow pulse"></div>
      </div>
    </div>
    <div class="card-url">{job.url}</div>
  {:else if job.metadata}
    <!-- Metadata loaded -->
    <div class="card-content">
      {#if job.metadata.thumbnailUrl}
        <div class="card-thumb">
          <img src={job.metadata.thumbnailUrl} alt={job.metadata.title} />
          {#if job.metadata.durationSeconds > 0}
            <span class="duration-badge">{formatDuration(job.metadata.durationSeconds)}</span>
          {/if}
        </div>
      {/if}
      <div class="card-info">
        <span class="card-title">{job.metadata.title}</span>
        <span class="card-uploader">{job.metadata.uploader}</span>
        <div class="card-meta-row">
          <span class="status-badge status-{job.status}">{statusLabels[job.status] ?? job.status}</span>
          {#if estimateSize(job)}
            <span class="size-estimate">~{estimateSize(job)}</span>
          {/if}
          {#if job.metadata.extractor}
            <span class="extractor-badge">{job.metadata.extractor}</span>
          {/if}
        </div>
      </div>
    </div>
  {:else}
    <!-- No metadata (fast-add or post-inspecting without data) -->
    <div class="card-content card-minimal">
      <div class="card-info">
        <span class="card-title">{job.url}</span>
        <div class="card-meta-row">
          <span class="status-badge status-{job.status}">{statusLabels[job.status] ?? job.status}</span>
        </div>
      </div>
    </div>
  {/if}

  <!-- Progress bar for active downloads -->
  {#if job.status === 'downloading' || job.status === 'processing'}
    <div class="card-progress">
      <div class="progress-track">
        <div class="progress-fill" style="width: {job.progress.percentage}%"></div>
      </div>
      <div class="progress-details">
        <span>{job.progress.percentage.toFixed(0)}%</span>
        {#if job.progress.downloadSpeed}
          <span>{job.progress.downloadSpeed}</span>
        {/if}
        {#if job.progress.eta}
          <span>ETA {job.progress.eta}</span>
        {/if}
        {#if job.progress.currentStep}
          <span class="step-label">{job.progress.currentStep}</span>
        {/if}
      </div>
    </div>
  {/if}

  {#if job.status === 'error' && job.errorMessage}
    <div class="card-error">
      <span class="error-icon">!</span>
      <span class="error-text">{job.errorMessage}</span>
    </div>
  {/if}
</div>

<style>
  .media-card {
    display: flex;
    flex-direction: column;
    gap: var(--spacing-sm);
    padding: var(--spacing-md);
    background-color: var(--card-bg);
    border: 2px solid transparent;
    border-radius: var(--radius-lg);
    cursor: pointer;
    transition: border-color 0.15s ease, box-shadow 0.15s ease;
  }

  .media-card:hover {
    border-color: var(--border-color);
  }

  .media-card.selected {
    border-color: var(--primary-color);
    box-shadow: 0 0 0 1px var(--primary-color);
  }

  .media-card.error {
    border-color: var(--error-color);
  }

  /* Skeleton loading */
  .card-skeleton {
    display: flex;
    gap: var(--spacing-md);
  }

  .skeleton-thumb {
    width: 120px;
    height: 68px;
    border-radius: var(--radius-sm);
    background-color: var(--bg-surface-hover);
    flex-shrink: 0;
  }

  .skeleton-info {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: var(--spacing-xs);
    justify-content: center;
  }

  .skeleton-line {
    height: 14px;
    border-radius: var(--radius-sm);
    background-color: var(--bg-surface-hover);
  }

  .skeleton-line.wide { width: 80%; }
  .skeleton-line.medium { width: 55%; }
  .skeleton-line.narrow { width: 30%; }

  .pulse {
    animation: pulse 1.5s ease-in-out infinite;
  }

  @keyframes pulse {
    0%, 100% { opacity: 0.4; }
    50% { opacity: 0.8; }
  }

  .card-url {
    font-size: 0.8rem;
    color: var(--text-muted);
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  /* Loaded content */
  .card-content {
    display: flex;
    gap: var(--spacing-md);
  }

  .card-thumb {
    position: relative;
    width: 120px;
    height: 68px;
    flex-shrink: 0;
    border-radius: var(--radius-sm);
    overflow: hidden;
    background-color: var(--bg-surface-hover);
  }

  .card-thumb img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  .duration-badge {
    position: absolute;
    bottom: 4px;
    right: 4px;
    background-color: rgba(0, 0, 0, 0.8);
    color: #fff;
    font-size: 0.7rem;
    padding: 1px 4px;
    border-radius: 3px;
    font-variant-numeric: tabular-nums;
  }

  .card-info {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 2px;
    min-width: 0;
  }

  .card-title {
    font-weight: 500;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .card-uploader {
    font-size: var(--font-size-sm);
    color: var(--text-muted);
  }

  .card-meta-row {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
    margin-top: 2px;
  }

  .status-badge {
    display: inline-block;
    font-size: 0.75rem;
    padding: 1px 6px;
    border-radius: var(--radius-sm);
    font-weight: 500;
  }

  .status-inspecting { background-color: rgba(0, 123, 255, 0.15); color: var(--primary-color); }
  .status-configuring { background-color: rgba(255, 193, 7, 0.15); color: var(--warning-color); }
  .status-queued { background-color: rgba(108, 117, 125, 0.15); color: var(--secondary-color); }
  .status-paused { background-color: rgba(108, 117, 125, 0.15); color: var(--secondary-color); }
  .status-downloading { background-color: rgba(0, 123, 255, 0.15); color: var(--primary-color); }
  .status-processing { background-color: rgba(0, 123, 255, 0.15); color: var(--primary-color); }
  .status-completed { background-color: rgba(40, 167, 69, 0.15); color: var(--success-color); }
  .status-error { background-color: rgba(220, 53, 69, 0.15); color: var(--error-color); }

  .size-estimate {
    font-size: 0.75rem;
    color: var(--text-muted);
  }

  .extractor-badge {
    font-size: 0.7rem;
    color: var(--text-muted);
    text-transform: capitalize;
  }

  .card-minimal {
    padding: var(--spacing-xs) 0;
  }

  /* Progress */
  .card-progress {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .progress-track {
    height: 4px;
    background-color: var(--border-color);
    border-radius: 2px;
    overflow: hidden;
  }

  .progress-fill {
    height: 100%;
    background-color: var(--primary-color);
    border-radius: 2px;
    transition: width 0.3s ease;
  }

  .progress-details {
    display: flex;
    gap: var(--spacing-sm);
    font-size: 0.75rem;
    color: var(--text-muted);
  }

  .step-label {
    margin-left: auto;
    font-style: italic;
  }

  .card-error {
    display: flex;
    align-items: flex-start;
    gap: var(--spacing-xs);
    font-size: 0.8rem;
    color: var(--error-color);
    padding: var(--spacing-xs) var(--spacing-sm);
    background-color: rgba(220, 53, 69, 0.08);
    border-radius: var(--radius-sm);
  }

  .error-icon {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 16px;
    height: 16px;
    border-radius: 50%;
    background-color: var(--error-color);
    color: #fff;
    font-size: 0.65rem;
    font-weight: 700;
    flex-shrink: 0;
    margin-top: 1px;
  }

  .error-text {
    overflow: hidden;
    display: -webkit-box;
    -webkit-line-clamp: 3;
    line-clamp: 3;
    -webkit-box-orient: vertical;
  }
</style>
