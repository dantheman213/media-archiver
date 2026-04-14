<script lang="ts">
  import { invoke } from '@tauri-apps/api/core';
  import { convertFileSrc } from '@tauri-apps/api/core';
  import { onMount } from 'svelte';
  import { history, clearHistory, removeHistoryRecord } from '../../stores/history';
  import type { HistoryRecord } from '../../types';

  let records: HistoryRecord[] = $state([]);

  $effect(() => {
    const unsub = history.subscribe((h) => (records = h));
    return unsub;
  });

  // Track which files exist on disk
  let fileExistsMap = $state<Record<string, boolean>>({});

  onMount(async () => {
    await checkFilesExist();
  });

  // Re-check when records change
  $effect(() => {
    if (records.length > 0) {
      checkFilesExist();
    }
  });

  async function checkFilesExist() {
    const map: Record<string, boolean> = {};
    const checks = records
      .filter(r => r.filePath)
      .map(async (r) => {
        try {
          const exists: boolean = await invoke('check_file_exists', { path: r.filePath });
          map[r.id] = exists;
        } catch {
          map[r.id] = false;
        }
      });
    await Promise.all(checks);
    fileExistsMap = map;
  }

  function formatDuration(seconds: number): string {
    const h = Math.floor(seconds / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const s = Math.floor(seconds % 60);
    if (h > 0) return `${h}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
    return `${m}:${String(s).padStart(2, '0')}`;
  }

  function formatFileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    if (bytes < 1024 * 1024 * 1024) return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
    return `${(bytes / (1024 * 1024 * 1024)).toFixed(2)} GB`;
  }

  function formatRelativeDate(iso: string): string {
    const date = new Date(iso);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMin = Math.floor(diffMs / 60000);
    const diffHr = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMin < 1) return 'Just now';
    if (diffMin < 60) return `${diffMin} min ago`;
    if (diffHr < 24) return `${diffHr}h ago`;
    if (diffDays === 1) return 'Yesterday';
    if (diffDays < 7) return `${diffDays} days ago`;
    return date.toLocaleDateString();
  }

  function formatAbsoluteDate(iso: string): string {
    return new Date(iso).toLocaleString();
  }

  // Search
  let searchQuery = $state('');

  // Sort
  type SortBy = 'date' | 'title' | 'size';
  let sortBy = $state<SortBy>('date');

  let filteredAndSorted = $derived.by(() => {
    let result = records;

    // Filter
    if (searchQuery.trim()) {
      const q = searchQuery.toLowerCase();
      result = result.filter(r =>
        r.title.toLowerCase().includes(q) ||
        r.uploader.toLowerCase().includes(q) ||
        r.url.toLowerCase().includes(q)
      );
    }

    // Sort
    result = [...result].sort((a, b) => {
      if (sortBy === 'date') {
        return new Date(b.completedAt).getTime() - new Date(a.completedAt).getTime();
      }
      if (sortBy === 'title') {
        return a.title.localeCompare(b.title);
      }
      if (sortBy === 'size') {
        return (b.fileSize ?? 0) - (a.fileSize ?? 0);
      }
      return 0;
    });

    return result;
  });

  async function handleOpen(path: string) {
    try {
      await invoke('open_file', { path });
    } catch (e) {
      console.error('Failed to open file:', e);
    }
  }

  async function handleShowInFolder(path: string) {
    try {
      await invoke('show_in_folder', { path });
    } catch (e) {
      console.error('Failed to show in folder:', e);
    }
  }

  async function handleClearAll() {
    await clearHistory();
    fileExistsMap = {};
  }

  async function handleRemove(id: string) {
    await removeHistoryRecord(id);
  }

  function getThumbSrc(record: HistoryRecord): string {
    if (record.cachedThumbnailPath) {
      return convertFileSrc(record.cachedThumbnailPath);
    }
    return record.thumbnailUrl || '';
  }
</script>

<div class="view">
  <header class="view-header">
    <h2>History</h2>
    <div class="header-actions">
      {#if records.length > 0}
        <select class="sort-select" bind:value={sortBy}>
          <option value="date">Newest First</option>
          <option value="title">By Title</option>
          <option value="size">By Size</option>
        </select>
        <button class="btn-secondary" onclick={handleClearAll}>Clear All</button>
      {/if}
    </div>
  </header>

  {#if records.length > 0}
    <div class="search-bar">
      <input
        type="text"
        class="search-input"
        placeholder="Search by title, uploader, or URL..."
        bind:value={searchQuery}
      />
    </div>
  {/if}

  {#if records.length === 0}
    <div class="empty-state">
      <p>No completed downloads yet.</p>
      <p class="hint">Completed downloads will appear here and persist across sessions.</p>
    </div>
  {:else if filteredAndSorted.length === 0}
    <div class="empty-state">
      <p>No results found.</p>
    </div>
  {:else}
    <ul class="history-list">
      {#each filteredAndSorted as record (record.id)}
        {@const fileExists = record.filePath ? (fileExistsMap[record.id] ?? true) : false}
        <li class="history-card">
          <div class="card-content">
            {#if getThumbSrc(record)}
              <div class="card-thumb">
                <img src={getThumbSrc(record)} alt={record.title} />
                {#if record.durationSeconds > 0}
                  <span class="duration-badge">{formatDuration(record.durationSeconds)}</span>
                {/if}
              </div>
            {:else}
              <div class="card-thumb placeholder-thumb"></div>
            {/if}
            <div class="card-info">
              <span class="card-title">{record.title}</span>
              {#if record.uploader}
                <span class="card-uploader">{record.uploader}</span>
              {/if}
              <div class="card-meta-row">
                <span class="format-badge">{record.formatLabel}</span>
                {#if record.extractor}
                  <span class="extractor-badge">{record.extractor}</span>
                {/if}
                {#if record.fileSize}
                  <span class="size-label">{formatFileSize(record.fileSize)}</span>
                {/if}
                <span class="date-label" title={formatAbsoluteDate(record.completedAt)}>
                  {formatRelativeDate(record.completedAt)}
                </span>
              </div>
              {#if record.filePath && !fileExists}
                <span class="file-missing">File moved or deleted</span>
              {/if}
            </div>
          </div>
          <div class="card-actions">
            {#if record.filePath && fileExists}
              <button class="btn-sm btn-action" onclick={() => handleOpen(record.filePath)} title="Open file">
                Open
              </button>
              <button class="btn-sm btn-action" onclick={() => handleShowInFolder(record.filePath)} title="Show in file explorer">
                Show in Folder
              </button>
            {/if}
            <button class="btn-sm btn-danger" onclick={() => handleRemove(record.id)}>Remove</button>
          </div>
        </li>
      {/each}
    </ul>
  {/if}
</div>

<style>
  .view {
    padding: var(--spacing-lg);
  }

  .view-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: var(--spacing-lg);
  }

  .view-header h2 {
    font-size: 1.5rem;
    font-weight: 700;
  }

  .header-actions {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
  }

  .sort-select {
    padding: var(--spacing-xs) var(--spacing-sm);
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-surface);
    color: var(--text-color);
    font-size: 0.85rem;
    cursor: pointer;
  }

  .search-bar {
    margin-bottom: var(--spacing-md);
  }

  .search-input {
    width: 100%;
    padding: var(--spacing-sm) var(--spacing-md);
    border: 2px solid var(--border-color);
    border-radius: var(--radius-md);
    background-color: var(--bg-surface);
    color: var(--text-color);
    font-size: 0.9rem;
    transition: border-color 0.15s ease;
  }

  .search-input:focus {
    outline: none;
    border-color: var(--primary-color);
  }

  .empty-state {
    text-align: center;
    padding: var(--spacing-xl) 0;
    color: var(--text-muted);
  }

  .hint {
    font-size: 0.85rem;
    opacity: 0.7;
    margin-top: var(--spacing-xs);
  }

  .history-list {
    list-style: none;
    display: flex;
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .history-card {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: var(--spacing-md);
    background-color: var(--bg-surface);
    border: 1px solid var(--border-color);
    border-radius: var(--radius-lg);
    gap: var(--spacing-md);
  }

  .card-content {
    display: flex;
    gap: var(--spacing-md);
    flex: 1;
    min-width: 0;
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

  .placeholder-thumb {
    display: flex;
    align-items: center;
    justify-content: center;
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
    flex-wrap: wrap;
  }

  .format-badge {
    display: inline-block;
    font-size: 0.75rem;
    padding: 1px 6px;
    border-radius: var(--radius-sm);
    font-weight: 500;
    background-color: rgba(40, 167, 69, 0.15);
    color: var(--success-color);
  }

  .extractor-badge {
    font-size: 0.7rem;
    color: var(--text-muted);
    text-transform: capitalize;
  }

  .size-label {
    font-size: 0.75rem;
    color: var(--text-muted);
  }

  .date-label {
    font-size: 0.75rem;
    color: var(--text-muted);
    cursor: default;
  }

  .file-missing {
    font-size: 0.75rem;
    color: var(--warning-color);
    margin-top: 2px;
  }

  .card-actions {
    display: flex;
    gap: var(--spacing-xs);
    flex-shrink: 0;
  }

  .btn-sm {
    padding: 2px var(--spacing-sm);
    font-size: 0.8rem;
    border: 1px solid var(--border-color);
    border-radius: var(--radius-sm);
    background-color: var(--bg-surface);
    color: var(--text-color);
    cursor: pointer;
    white-space: nowrap;
  }

  .btn-sm:hover {
    background-color: var(--bg-surface-hover);
  }

  .btn-action {
    color: var(--primary-color);
    border-color: var(--primary-color);
  }

  .btn-action:hover {
    background-color: rgba(0, 123, 255, 0.1);
  }

  .btn-danger {
    color: var(--error-color);
    border-color: var(--error-color);
  }

  .btn-danger:hover {
    background-color: rgba(220, 53, 69, 0.1);
  }

  .btn-secondary {
    padding: var(--spacing-xs) var(--spacing-md);
    background-color: var(--secondary-color);
    color: #fff;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 0.85rem;
  }

  .btn-secondary:hover {
    opacity: 0.85;
  }
</style>
