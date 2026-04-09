<script lang="ts">
  import { invoke } from '@tauri-apps/api/core';
  import { onMount } from 'svelte';
  import {
    settings,
    toggleGlobalPause,
    setConcurrentDownloads,
    toggleAutoRetry,
    setDownloadPath,
    setTheme,
  } from '../../stores/settings';

  let defaultPath = $state('');

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

    <label class="setting">
      <span class="setting-label">Concurrent Downloads</span>
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
    </label>

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

    <label class="setting">
      <span class="setting-label">Auto-Retry Failed Downloads</span>
      <button
        class="toggle"
        class:active={$settings.autoRetry}
        onclick={toggleAutoRetry}
        role="switch"
        aria-checked={$settings.autoRetry}
      >
        {$settings.autoRetry ? 'On' : 'Off'}
      </button>
    </label>

    <label class="setting">
      <span class="setting-label">Global Pause</span>
      <button
        class="toggle"
        class:active={$settings.globalPaused}
        onclick={toggleGlobalPause}
        role="switch"
        aria-checked={$settings.globalPaused}
      >
        {$settings.globalPaused ? 'Paused' : 'Running'}
      </button>
    </label>
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
</style>
