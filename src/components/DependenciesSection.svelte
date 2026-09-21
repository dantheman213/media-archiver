<script lang="ts">
  import { onMount } from 'svelte';
  import { get } from 'svelte/store';
  import { invoke } from '@tauri-apps/api/core';
  import {
    binaryStatus,
    binaryErrorMsg,
    checkBinaries,
    checkYtDlpUpdate,
    ytdlpUpdateInfo,
    ytdlpChecking,
    ytdlpUpdating,
    ytdlpUpdateError,
    ytdlpUpdateMessage,
    performYtDlpUpdate,
    loadBinaryPaths,
    saveBinaryPaths,
    type BinaryPaths,
  } from '../stores/binaries';

  // Draft path overrides. An empty value means "auto-detect this dependency".
  let paths = $state<BinaryPaths>({});
  let checking = $state(false);
  let savingPaths = $state(false);
  let pathsMessage = $state('');

  onMount(async () => {
    try {
      paths = await loadBinaryPaths();
    } catch (e) {
      console.error('Failed to load binary paths:', e);
    }
  });

  async function recheck() {
    checking = true;
    try {
      await checkBinaries({ silent: true });
    } finally {
      checking = false;
    }
  }

  async function browse(key: keyof BinaryPaths) {
    try {
      const selected = await invoke<string | null>('pick_file');
      if (selected) {
        paths = { ...paths, [key]: selected };
        pathsMessage = '';
      }
    } catch (e) {
      console.error('Failed to pick file:', e);
    }
  }

  function clearOverride(key: keyof BinaryPaths) {
    paths = { ...paths, [key]: '' };
    pathsMessage = '';
  }

  async function applyPaths() {
    savingPaths = true;
    pathsMessage = '';
    try {
      await saveBinaryPaths(paths);
      paths = await loadBinaryPaths();
      const status = get(binaryStatus);
      const allFound =
        !!status &&
        status.yt_dlp_found &&
        status.ffmpeg_found &&
        status.ffprobe_found &&
        status.atomic_parsley_found;
      pathsMessage = allFound ? 'Saved.' : 'Saved. Some dependencies are still missing.';
    } catch (e) {
      pathsMessage = `Couldn't save: ${String(e)}`;
    } finally {
      savingPaths = false;
    }
  }
</script>

{#snippet pathRow(key: keyof BinaryPaths, autoPath?: string)}
  <div class="path-row">
    <input
      type="text"
      class="text-input"
      value={paths[key] ?? ''}
      placeholder={autoPath ? `Auto-detected: ${autoPath}` : 'Auto-detect'}
      oninput={(e) => {
        paths = { ...paths, [key]: e.currentTarget.value };
        pathsMessage = '';
      }}
    />
    <button class="btn-secondary" onclick={() => browse(key)}>Browse</button>
    {#if paths[key]}
      <button class="btn-secondary" onclick={() => clearOverride(key)}>Use Auto</button>
    {/if}
  </div>
{/snippet}

<section class="settings-group">
  <div class="group-header">
    <div>
      <h3>Dependencies</h3>
      <p class="group-desc">
        The helper tools that power downloads. Leave a path empty to let Media Archiver use its
        managed copy or one found on your system PATH.
      </p>
    </div>
    <button class="btn-secondary" onclick={recheck} disabled={checking}>
      {checking ? 'Checking...' : 'Re-check'}
    </button>
  </div>

  {#if $binaryErrorMsg}
    <div class="error-banner">{$binaryErrorMsg}</div>
  {/if}

  {#if !$binaryStatus}
    <p class="muted">Checking for dependencies...</p>
  {:else}
    <div class="binary-list">
      <div
        class="binary-card"
        class:found={$binaryStatus.yt_dlp_found}
        class:missing={!$binaryStatus.yt_dlp_found}
      >
        <div class="binary-card-header">
          <div class="binary-title">
            <h4>yt-dlp</h4>
            <span class="status-badge">{$binaryStatus.yt_dlp_found ? 'Found' : 'Not Found'}</span>
          </div>
          {#if $binaryStatus.yt_dlp_found}
            <div class="card-actions">
              <button
                class="btn-sm"
                onclick={checkYtDlpUpdate}
                disabled={$ytdlpChecking || $ytdlpUpdating}
              >
                {$ytdlpChecking ? 'Checking...' : 'Check for updates'}
              </button>
              {#if $ytdlpUpdateInfo?.updateAvailable}
                <button class="btn-sm btn-update" onclick={performYtDlpUpdate} disabled={$ytdlpUpdating}>
                  {$ytdlpUpdating ? 'Updating...' : 'Update yt-dlp'}
                </button>
              {/if}
            </div>
          {/if}
        </div>

        {#if $binaryStatus.yt_dlp_version || $ytdlpUpdateInfo?.updateAvailable}
          <div class="binary-meta">
            {#if $binaryStatus.yt_dlp_version}
              <span class="version">{$binaryStatus.yt_dlp_version}</span>
            {/if}
            {#if $ytdlpUpdateInfo?.updateAvailable}
              <span class="version-new">{$ytdlpUpdateInfo.latestVersion}</span>
            {/if}
          </div>
        {/if}

        {#if $ytdlpUpdateError}
          <p class="update-message update-error">Update check failed: {$ytdlpUpdateError}</p>
        {:else if $ytdlpUpdateInfo && !$ytdlpUpdateInfo.updateAvailable}
          <p class="update-message">Up to date ({$ytdlpUpdateInfo.latestVersion}).</p>
        {/if}
        {#if $ytdlpUpdateMessage}
          <p
            class="update-message"
            class:update-error={$ytdlpUpdateMessage.startsWith('Update failed')}
          >
            {$ytdlpUpdateMessage}
          </p>
        {/if}

        {@render pathRow('yt_dlp_path', $binaryStatus.yt_dlp_path)}
      </div>

      <div
        class="binary-card"
        class:found={$binaryStatus.ffmpeg_found}
        class:missing={!$binaryStatus.ffmpeg_found}
      >
        <div class="binary-card-header">
          <div class="binary-title">
            <h4>ffmpeg</h4>
            <span class="status-badge">{$binaryStatus.ffmpeg_found ? 'Found' : 'Not Found'}</span>
          </div>
        </div>
        {#if $binaryStatus.ffmpeg_version}
          <p class="version">{$binaryStatus.ffmpeg_version}</p>
        {/if}
        {@render pathRow('ffmpeg_path', $binaryStatus.ffmpeg_path)}
      </div>

      <div
        class="binary-card"
        class:found={$binaryStatus.ffprobe_found}
        class:missing={!$binaryStatus.ffprobe_found}
      >
        <div class="binary-card-header">
          <div class="binary-title">
            <h4>ffprobe</h4>
            <span class="status-badge">{$binaryStatus.ffprobe_found ? 'Found' : 'Not Found'}</span>
          </div>
        </div>
        {#if $binaryStatus.ffprobe_version}
          <p class="version">{$binaryStatus.ffprobe_version}</p>
        {/if}
        {#if !$binaryStatus.ffprobe_found && !paths.ffprobe_path}
          <p class="update-message update-error">
            Required for merging and embedding. It usually ships alongside ffmpeg.
          </p>
        {/if}
        {@render pathRow('ffprobe_path', $binaryStatus.ffprobe_path)}
      </div>

      <div
        class="binary-card"
        class:found={$binaryStatus.atomic_parsley_found}
        class:missing={!$binaryStatus.atomic_parsley_found}
      >
        <div class="binary-card-header">
          <div class="binary-title">
            <h4>AtomicParsley</h4>
            <span class="status-badge">
              {$binaryStatus.atomic_parsley_found ? 'Found' : 'Not Found'}
            </span>
          </div>
        </div>
        {#if $binaryStatus.atomic_parsley_version}
          <p class="version">{$binaryStatus.atomic_parsley_version}</p>
        {/if}
        {@render pathRow('atomic_parsley_path', $binaryStatus.atomic_parsley_path)}
      </div>
    </div>

    <div class="paths-footer">
      <span class="paths-message">{pathsMessage}</span>
      <button class="btn-secondary" onclick={applyPaths} disabled={savingPaths}>
        {savingPaths ? 'Saving...' : 'Save Paths'}
      </button>
    </div>
  {/if}
</section>

<style>
  .settings-group {
    margin-bottom: var(--spacing-xl);
  }

  .group-header {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    gap: var(--spacing-md);
    padding-bottom: var(--spacing-xs);
    border-bottom: 1px solid var(--border-color);
    margin-bottom: var(--spacing-md);
  }

  .group-header h3 {
    font-size: 1.1rem;
    font-weight: 600;
  }

  .group-desc {
    margin-top: var(--spacing-xs);
    font-size: 0.8rem;
    color: var(--text-muted);
    max-width: 60ch;
  }

  .error-banner {
    background-color: rgba(220, 53, 69, 0.1);
    color: var(--error-color);
    padding: var(--spacing-sm) var(--spacing-md);
    border-radius: 6px;
    margin-bottom: var(--spacing-md);
    border: 1px solid var(--error-color);
    font-size: 0.85rem;
  }

  .muted {
    color: var(--text-muted);
    font-size: 0.9rem;
  }

  .binary-list {
    display: grid;
    grid-template-columns: 1fr;
    gap: var(--spacing-md);
  }

  .binary-card {
    padding: var(--spacing-md);
    border-radius: 8px;
    border: 1px solid var(--border-color);
    background-color: var(--bg-surface);
  }

  .binary-card.found {
    border-color: var(--success-color);
  }

  .binary-card.missing {
    border-color: var(--error-color);
  }

  .binary-card-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: var(--spacing-sm);
  }

  .binary-title {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
  }

  .binary-title h4 {
    font-size: 1rem;
    font-weight: 600;
  }

  .status-badge {
    display: inline-block;
    padding: 2px 10px;
    border-radius: 10px;
    font-size: 0.75rem;
    font-weight: 600;
  }

  .found .status-badge {
    background-color: rgba(40, 167, 69, 0.15);
    color: var(--success-color);
  }

  .missing .status-badge {
    background-color: rgba(220, 53, 69, 0.15);
    color: var(--error-color);
  }

  .card-actions {
    display: flex;
    gap: var(--spacing-sm);
  }

  .binary-meta {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
    margin-top: var(--spacing-xs);
  }

  .version {
    font-size: 0.85rem;
    color: var(--text-color);
    font-family: monospace;
    margin-top: var(--spacing-xs);
  }

  .binary-meta .version {
    margin-top: 0;
  }

  .version-new {
    display: inline-block;
    padding: 1px 8px;
    border-radius: 10px;
    font-size: 0.75rem;
    font-weight: 600;
    font-family: monospace;
    background-color: rgba(40, 167, 69, 0.15);
    color: var(--success-color);
  }

  .update-message {
    margin-top: var(--spacing-xs);
    font-size: 0.8rem;
    color: var(--success-color);
    white-space: pre-line;
  }

  .update-error {
    color: var(--error-color);
  }

  .path-row {
    display: flex;
    gap: var(--spacing-sm);
    margin-top: var(--spacing-sm);
  }

  .text-input {
    flex: 1;
    padding: var(--spacing-sm);
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-surface);
    color: var(--text-color);
    font-size: 0.85rem;
  }

  .text-input::placeholder {
    color: var(--text-muted);
  }

  .paths-footer {
    display: flex;
    align-items: center;
    justify-content: flex-end;
    gap: var(--spacing-md);
    margin-top: var(--spacing-md);
  }

  .paths-message {
    font-size: 0.8rem;
    color: var(--text-muted);
  }

  .btn-secondary {
    padding: var(--spacing-xs) var(--spacing-md);
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-surface);
    color: var(--text-color);
    cursor: pointer;
    font-weight: 600;
    font-size: 0.85rem;
  }

  .btn-secondary:hover:not(:disabled) {
    background-color: var(--bg-surface-hover);
  }

  .btn-secondary:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .btn-sm {
    padding: 2px var(--spacing-sm);
    font-size: 0.8rem;
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-surface);
    color: var(--text-color);
    cursor: pointer;
  }

  .btn-sm:hover:not(:disabled) {
    background-color: var(--bg-surface-hover);
  }

  .btn-sm:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .btn-update {
    background-color: var(--primary-color);
    border-color: var(--primary-color);
    color: #fff;
  }

  .btn-update:hover:not(:disabled) {
    opacity: 0.9;
    background-color: var(--primary-color);
  }
</style>
