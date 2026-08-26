<script lang="ts">
  import { invoke } from '@tauri-apps/api/core';
  import {
    binaryStatus,
    binaryCheckState,
    binaryErrorMsg,
    checkBinaries,
    checkYtDlpUpdate,
    ytdlpUpdateInfo,
    ytdlpChecking,
    ytdlpUpdateError
  } from '../../stores/binaries';

  let updating = $state(false);
  let updateMessage = $state('');

  async function updateYtDlp() {
    updating = true;
    updateMessage = '';
    try {
      const msg: string = await invoke('update_ytdlp');
      updateMessage = msg;
    } catch (e) {
      updateMessage = `Update failed: ${String(e)}`;
    } finally {
      updating = false;
      // Re-check binaries to refresh version info, then re-check for updates.
      await checkBinaries();
      checkYtDlpUpdate();
    }
  }
</script>

<div class="view">
  <header class="view-header">
    <h2>Dependencies</h2>
    <button class="btn-primary" onclick={checkBinaries} disabled={$binaryCheckState === 'checking'}>
      {$binaryCheckState === 'checking' ? 'Checking...' : 'Re-check'}
    </button>
  </header>

  {#if $binaryErrorMsg}
    <div class="error-banner">{$binaryErrorMsg}</div>
  {/if}

  {#if $binaryCheckState === 'checking' && !$binaryStatus}
    <p class="muted">Checking for binaries...</p>
  {:else if $binaryStatus}
    <div class="binary-grid">
      <div class="binary-card" class:found={$binaryStatus.yt_dlp_found} class:missing={!$binaryStatus.yt_dlp_found}>
        <div class="binary-card-header">
          <h3>yt-dlp</h3>
          {#if $binaryStatus.yt_dlp_found}
            <div class="card-actions">
              <button class="btn-sm" onclick={checkYtDlpUpdate} disabled={$ytdlpChecking || updating}>
                {$ytdlpChecking ? 'Checking...' : 'Check for updates'}
              </button>
              <button class="btn-sm" onclick={updateYtDlp} disabled={updating}>
                {updating ? 'Updating...' : 'Update yt-dlp'}
              </button>
            </div>
          {/if}
        </div>
        <span class="status-badge">{$binaryStatus.yt_dlp_found ? 'Found' : 'Not Found'}</span>
        {#if $binaryStatus.yt_dlp_version}
          <p class="version">{$binaryStatus.yt_dlp_version}</p>
        {/if}
        {#if $binaryStatus.yt_dlp_path}
          <p class="path">{$binaryStatus.yt_dlp_path}</p>
        {/if}
        {#if $ytdlpUpdateError}
          <p class="update-message update-error">Update check failed: {$ytdlpUpdateError}</p>
        {:else if $ytdlpUpdateInfo}
          {#if $ytdlpUpdateInfo.updateAvailable}
            <p class="update-message">Update available: {$ytdlpUpdateInfo.currentVersion} &rarr; {$ytdlpUpdateInfo.latestVersion}</p>
          {:else}
            <p class="update-message">Up to date ({$ytdlpUpdateInfo.latestVersion}).</p>
          {/if}
        {/if}
        {#if updateMessage}
          <p class="update-message" class:update-error={updateMessage.startsWith('Update failed')}>{updateMessage}</p>
        {/if}
      </div>

      <div class="binary-card" class:found={$binaryStatus.ffmpeg_found} class:missing={!$binaryStatus.ffmpeg_found}>
        <h3>ffmpeg</h3>
        <span class="status-badge">{$binaryStatus.ffmpeg_found ? 'Found' : 'Not Found'}</span>
        {#if $binaryStatus.ffmpeg_version}
          <p class="version">{$binaryStatus.ffmpeg_version}</p>
        {/if}
        {#if $binaryStatus.ffmpeg_path}
          <p class="path">{$binaryStatus.ffmpeg_path}</p>
        {/if}
      </div>

      <div class="binary-card" class:found={$binaryStatus.ffprobe_found} class:missing={!$binaryStatus.ffprobe_found}>
        <h3>ffprobe</h3>
        <span class="status-badge">{$binaryStatus.ffprobe_found ? 'Found' : 'Not Found'}</span>
        {#if $binaryStatus.ffprobe_version}
          <p class="version">{$binaryStatus.ffprobe_version}</p>
        {/if}
        {#if $binaryStatus.ffprobe_path}
          <p class="path">{$binaryStatus.ffprobe_path}</p>
        {/if}
        {#if !$binaryStatus.ffprobe_found}
          <p class="update-message update-error">Required for merging and embedding. Use Automatic Setup to install it.</p>
        {/if}
      </div>

      <div class="binary-card" class:found={$binaryStatus.atomic_parsley_found} class:missing={!$binaryStatus.atomic_parsley_found}>
        <h3>AtomicParsley</h3>
        <span class="status-badge">{$binaryStatus.atomic_parsley_found ? 'Found' : 'Not Found'}</span>
        {#if $binaryStatus.atomic_parsley_version}
          <p class="version">{$binaryStatus.atomic_parsley_version}</p>
        {/if}
        {#if $binaryStatus.atomic_parsley_path}
          <p class="path">{$binaryStatus.atomic_parsley_path}</p>
        {/if}
      </div>
    </div>
  {/if}
</div>

<style>
  .view {
    padding: var(--spacing-lg);
    height: 100%;
    overflow-y: auto;
    box-sizing: border-box;
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

  .error-banner {
    background-color: rgba(220, 53, 69, 0.1);
    color: var(--error-color);
    padding: var(--spacing-md);
    border-radius: 6px;
    margin-bottom: var(--spacing-md);
    border: 1px solid var(--error-color);
  }

  .muted {
    color: var(--secondary-color);
  }

  .binary-grid {
    display: grid;
    grid-template-columns: 1fr;
    gap: var(--spacing-md);
  }

  .binary-card {
    padding: var(--spacing-lg);
    border-radius: 8px;
    border: 1px solid var(--border-color);
    background-color: var(--bg-surface);
  }

  .binary-card h3 {
    font-size: 1.1rem;
    margin-bottom: var(--spacing-sm);
  }

  .binary-card.found {
    border-color: var(--success-color);
  }

  .binary-card.missing {
    border-color: var(--error-color);
  }

  .status-badge {
    display: inline-block;
    padding: 2px 10px;
    border-radius: 10px;
    font-size: 0.8rem;
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

  .version {
    margin-top: var(--spacing-xs);
    font-size: 0.85rem;
    color: var(--text-color);
    font-family: monospace;
  }

  .path {
    margin-top: var(--spacing-xs);
    font-size: 0.8rem;
    color: var(--secondary-color);
    word-break: break-all;
  }

  .btn-primary {
    padding: var(--spacing-xs) var(--spacing-md);
    background-color: var(--primary-color);
    color: #fff;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 0.85rem;
  }

  .btn-primary:hover {
    opacity: 0.9;
  }

  .btn-primary:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .binary-card-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: var(--spacing-sm);
  }

  .binary-card-header h3 {
    margin-bottom: 0;
  }

  .card-actions {
    display: flex;
    gap: var(--spacing-sm);
  }

  .btn-sm {
    padding: 2px var(--spacing-sm);
    font-size: 0.8rem;
    border: 1px solid var(--border-color);
    border-radius: var(--radius-sm, 4px);
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

  .update-message {
    margin-top: var(--spacing-sm);
    font-size: 0.8rem;
    color: var(--success-color);
    white-space: pre-line;
  }

  .update-error {
    color: var(--error-color);
  }
</style>
