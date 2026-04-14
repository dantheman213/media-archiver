<script lang="ts">
  import { invoke } from '@tauri-apps/api/core';
  import { goto } from '$app/navigation';
  import { onMount } from 'svelte';
  import {
    settings,
    toggleGlobalPause,
    setConcurrentDownloads,
    toggleAutoRetry,
    setDownloadPath,
    setTheme,
    resetSettings,
  } from '../../stores/settings';
  import { clearHistory } from '../../stores/history';
  import { clearMetadataCache } from '../../stores/metadataCache';

  let defaultPath = $state('');
  let confirmingClearHistory = $state(false);
  let confirmingClearCache = $state(false);
  let confirmingResetSettings = $state(false);

  onMount(async () => {
    try {
      defaultPath = await invoke('get_default_download_dir') || '';
    } catch (e) {
      console.error("Failed to fetch default download dir:", e);
    }
  });

  async function handleBrowse() {
    try {
      const selected = await invoke<string | null>('select_directory');
      if (selected) {
        setDownloadPath(selected);
      }
    } catch (e) {
      console.error("Failed to pick folder:", e);
    }
  }

  async function handleClearHistory() {
    await clearHistory();
    confirmingClearHistory = false;
  }

  async function handleClearCache() {
    await clearMetadataCache();
    try {
      await invoke('clear_thumbnail_cache');
    } catch (e) {
      console.error('Failed to clear thumbnail cache:', e);
    }
    confirmingClearCache = false;
  }

  async function handleResetSettings() {
    await resetSettings();
    confirmingResetSettings = false;
  }
</script>

<div class="view">
  <header class="view-header">
    <h2>Settings</h2>
  </header>

  <section class="settings-group">
    <h3>Appearance</h3>

    <label class="setting">
      <span class="setting-label">Theme</span>
      <div class="setting-control">
        <select
          class="select-input"
          value={$settings.theme}
          onchange={(e) => setTheme(e.currentTarget.value as any)}
        >
          <option value="system">OS Default</option>
          <option value="light">Light</option>
          <option value="dark">Dark</option>
        </select>
      </div>
    </label>
  </section>

  <section class="settings-group">
    <h3>Downloads</h3>

    <div class="setting">
      <div>
        <span class="setting-label">Concurrent Downloads</span>
        <span class="setting-desc">Number of files to download at the same time</span>
      </div>
      <div class="setting-control">
        <input
          type="range"
          min="1"
          max="5"
          value={$settings.concurrentDownloads}
          oninput={(e) => setConcurrentDownloads(parseInt(e.currentTarget.value))}
        />
        <span class="setting-value">{$settings.concurrentDownloads}</span>
      </div>
    </div>

    <label class="setting">
      <span class="setting-label">Download Folder</span>
      <div class="path-input-group">
        <input
          type="text"
          class="text-input"
          value={$settings.downloadPath}
          onchange={(e) => setDownloadPath(e.currentTarget.value)}
          placeholder={defaultPath || "Select a folder..."}
        />
        <button class="btn-secondary" onclick={handleBrowse}>Browse</button>
      </div>
    </label>
  </section>

  <section class="settings-group">
    <h3>Behavior</h3>

    <div class="setting">
      <div>
        <span class="setting-label">Auto-Retry Failed Downloads</span>
        <span class="setting-desc">Automatically retry downloads that fail due to temporary errors</span>
      </div>
      <button
        class="toggle"
        class:active={$settings.autoRetry}
        onclick={toggleAutoRetry}
        role="switch"
        aria-checked={$settings.autoRetry}
      >
        {$settings.autoRetry ? 'On' : 'Off'}
      </button>
    </div>

    <div class="setting">
      <div>
        <span class="setting-label">Global Pause</span>
        <span class="setting-desc">Pause all active and pending downloads</span>
      </div>
      <button
        class="toggle"
        class:active={$settings.globalPaused}
        onclick={toggleGlobalPause}
        role="switch"
        aria-checked={$settings.globalPaused}
      >
        {$settings.globalPaused ? 'Paused' : 'Running'}
      </button>
    </div>
  </section>

  <section class="settings-group">
    <h3>Data & Storage</h3>

    <div class="setting">
      <div>
        <span class="setting-label">Download History</span>
        <span class="setting-desc">Remove all records of past downloads</span>
      </div>
      {#if confirmingClearHistory}
        <div class="confirm-group">
          <span class="confirm-label">Are you sure?</span>
          <button class="btn-danger-sm" onclick={handleClearHistory}>Yes, Clear</button>
          <button class="btn-cancel-sm" onclick={() => confirmingClearHistory = false}>Cancel</button>
        </div>
      {:else}
        <button class="btn-destructive" onclick={() => confirmingClearHistory = true}>Clear History</button>
      {/if}
    </div>

    <div class="setting">
      <div>
        <span class="setting-label">Cached Data</span>
        <span class="setting-desc">Clear cached metadata and thumbnails for previously seen URLs</span>
      </div>
      {#if confirmingClearCache}
        <div class="confirm-group">
          <span class="confirm-label">Are you sure?</span>
          <button class="btn-danger-sm" onclick={handleClearCache}>Yes, Clear</button>
          <button class="btn-cancel-sm" onclick={() => confirmingClearCache = false}>Cancel</button>
        </div>
      {:else}
        <button class="btn-destructive" onclick={() => confirmingClearCache = true}>Clear Cache</button>
      {/if}
    </div>

    <div class="setting">
      <div>
        <span class="setting-label">Reset Settings</span>
        <span class="setting-desc">Restore all settings to their defaults</span>
      </div>
      {#if confirmingResetSettings}
        <div class="confirm-group">
          <span class="confirm-label">Are you sure?</span>
          <button class="btn-danger-sm" onclick={handleResetSettings}>Yes, Reset</button>
          <button class="btn-cancel-sm" onclick={() => confirmingResetSettings = false}>Cancel</button>
        </div>
      {:else}
        <button class="btn-destructive" onclick={() => confirmingResetSettings = true}>Reset All</button>
      {/if}
    </div>
  </section>

  <section class="settings-group">
    <h3>Advanced</h3>
    <div class="setting">
      <div>
        <span class="setting-label">Dependencies</span>
        <span class="setting-desc">Check status or update the tools that power downloads</span>
      </div>
      <button class="btn-link" onclick={() => goto('/binaries')}>Manage</button>
    </div>
  </section>
</div>

<style>
  .view {
    padding: var(--spacing-lg);
    max-width: 640px;
  }

  .view-header {
    margin-bottom: var(--spacing-lg);
  }

  .view-header h2 {
    font-size: 1.5rem;
    font-weight: 700;
  }

  .settings-group {
    margin-bottom: var(--spacing-xl);
  }

  .settings-group h3 {
    font-size: 1.1rem;
    font-weight: 600;
    margin-bottom: var(--spacing-md);
    padding-bottom: var(--spacing-xs);
    border-bottom: 1px solid var(--border-color);
  }

  .setting {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: var(--spacing-sm) 0;
    gap: var(--spacing-md);
  }

  .setting-label {
    font-weight: 500;
  }

  .setting-control {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
  }

  .setting-value {
    min-width: 24px;
    text-align: center;
    font-weight: 600;
  }

  .text-input {
    flex: 1;
    padding: var(--spacing-sm);
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-surface);
    color: var(--text-color);
    font-size: 0.9rem;
  }

  .select-input {
    padding: var(--spacing-sm);
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-surface);
    color: var(--text-color);
    font-size: 0.9rem;
    cursor: pointer;
  }

  .path-input-group {
    display: flex;
    gap: var(--spacing-sm);
    flex: 1;
  }

  .path-input-group .text-input {
    flex: 1;
  }

  .btn-secondary {
    padding: var(--spacing-sm) var(--spacing-md);
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-surface);
    color: var(--text-color);
    cursor: pointer;
    font-weight: 600;
    font-size: 0.9rem;
  }

  .btn-secondary:hover {
    background-color: var(--bg-surface-hover);
  }

  input[type='range'] {
    width: 140px;
    accent-color: var(--primary-color);
  }

  .toggle {
    padding: var(--spacing-xs) var(--spacing-md);
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-surface);
    color: var(--text-color);
    cursor: pointer;
    font-size: 0.85rem;
    font-weight: 500;
    min-width: 72px;
    transition: background-color 0.15s ease, border-color 0.15s ease;
  }

  .toggle.active {
    background-color: var(--primary-color);
    border-color: var(--primary-color);
    color: #fff;
  }

  .setting-desc {
    display: block;
    font-size: 0.8rem;
    color: var(--text-muted);
    margin-top: 2px;
  }

  .btn-destructive {
    padding: var(--spacing-xs) var(--spacing-md);
    border: 1px solid var(--error-color);
    border-radius: 4px;
    background-color: transparent;
    color: var(--error-color);
    cursor: pointer;
    font-size: 0.85rem;
    font-weight: 500;
    transition: background-color 0.15s ease;
  }

  .btn-destructive:hover {
    background-color: rgba(220, 53, 69, 0.1);
  }

  .confirm-group {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
  }

  .confirm-label {
    font-size: 0.85rem;
    color: var(--text-muted);
  }

  .btn-danger-sm {
    padding: var(--spacing-xs) var(--spacing-sm);
    border: none;
    border-radius: 4px;
    background-color: var(--error-color);
    color: #fff;
    cursor: pointer;
    font-size: 0.8rem;
    font-weight: 600;
  }

  .btn-danger-sm:hover {
    opacity: 0.9;
  }

  .btn-cancel-sm {
    padding: var(--spacing-xs) var(--spacing-sm);
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-surface);
    color: var(--text-muted);
    cursor: pointer;
    font-size: 0.8rem;
  }

  .btn-cancel-sm:hover {
    background-color: var(--bg-surface-hover);
  }

  .btn-link {
    padding: var(--spacing-xs) var(--spacing-md);
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-surface);
    color: var(--primary-color);
    cursor: pointer;
    font-size: 0.85rem;
    font-weight: 500;
    transition: background-color 0.15s ease;
  }

  .btn-link:hover {
    background-color: var(--bg-surface-hover);
  }
</style>
