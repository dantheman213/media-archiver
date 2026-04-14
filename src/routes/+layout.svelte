<script lang="ts">
  import '../app.css';
  import Onboarding from '../components/Onboarding.svelte';
  import ErrorToast from '../components/ErrorToast.svelte';
  import { page } from '$app/stores';
  import { goto } from '$app/navigation';
  import { toggleGlobalPause, settings, initializeSettings } from '../stores/settings';
  import { selectedJobId, removeJob, jobs } from '../stores/queue';
  import { checkBinaries, binaryCheckState, checkYtDlpUpdate, performYtDlpUpdate, ytdlpUpdateInfo, ytdlpUpdating } from '../stores/binaries';
  import { loadHistory } from '../stores/history';
  import { loadMetadataCache } from '../stores/metadataCache';
  import type { NavRoute } from '../types';
  import { onMount } from 'svelte';
  import { getVersion } from '@tauri-apps/api/app';
  import { getCurrentWindow } from '@tauri-apps/api/window';

  let { children } = $props();

  let appVersion = $state('');
  let updateCheckTriggered = $state(false);

  interface NavItem {
    route: NavRoute;
    label: string;
    path: string;
  }

  const navItems: NavItem[] = [
    { route: 'queue', label: 'Queue', path: '/' },
    { route: 'history', label: 'History', path: '/history' },
    { route: 'settings', label: 'Settings', path: '/settings' },
  ];

  function isActive(itemPath: string, currentPath: string): boolean {
    if (itemPath === '/') return currentPath === '/';
    return currentPath.startsWith(itemPath);
  }

  function handleKeydown(event: KeyboardEvent) {
    // Ignore shortcuts when typing in inputs
    const tag = (event.target as HTMLElement)?.tagName;
    if (tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT') return;

    if (event.code === 'Space') {
      event.preventDefault();
      toggleGlobalPause();
    }

    if (event.code === 'Delete') {
      event.preventDefault();
      const id = $selectedJobId;
      if (id) {
        removeJob(id);
      }
    }
  }

  onMount(async () => {
    checkBinaries();
    initializeSettings();
    loadHistory();
    loadMetadataCache();
    const version = await getVersion();
    appVersion = version;
    await getCurrentWindow().setTitle(`Media Archiver v${version}`);
  });

  $effect(() => {
    if ($binaryCheckState === 'done' && !updateCheckTriggered) {
      updateCheckTriggered = true;
      checkYtDlpUpdate();
    }
  });

  $effect(() => {
    if ($settings.theme === 'light' || $settings.theme === 'dark') {
      document.documentElement.setAttribute('data-theme', $settings.theme);
    } else {
      document.documentElement.removeAttribute('data-theme');
    }
  });

  let downloadingCount = $derived(
    $jobs.filter(j => j.status === 'downloading' || j.status === 'processing').length
  );
</script>

<svelte:window onkeydown={handleKeydown} />

<ErrorToast />

<Onboarding />

<div class="app-shell">
  <aside class="sidebar">
    <div class="sidebar-header">
      <h1>Media Archiver</h1>
    </div>
    <ul class="sidebar-nav">
      {#each navItems as item}
        <li>
          <button
            class:active={isActive(item.path, $page.url.pathname)}
            onclick={() => goto(item.path)}
          >
            {item.label}
          </button>
        </li>
      {/each}
    </ul>
    <div class="sidebar-footer">
      {#if $binaryCheckState === 'checking'}
        <span class="footer-status">Starting up...</span>
      {:else if $settings.globalPaused}
        <span class="footer-status">Paused</span>
      {:else if downloadingCount > 0}
        <span class="footer-status">{downloadingCount} downloading</span>
      {:else}
        <span class="footer-status">Ready</span>
      {/if}
      <span class="footer-version">{appVersion ? `v${appVersion}` : ''}</span>
      {#if $ytdlpUpdateInfo?.updateAvailable}
        <button
          class="update-banner"
          onclick={performYtDlpUpdate}
          disabled={$ytdlpUpdating}
        >
          {$ytdlpUpdating ? 'Updating yt-dlp...' : `yt-dlp ${$ytdlpUpdateInfo.latestVersion} available`}
        </button>
      {/if}
    </div>
  </aside>
  <main class="main-content">
    {@render children()}
  </main>
</div>

<style>
  .footer-status {
    display: block;
  }
  .footer-version {
    display: block;
    font-size: 0.75rem;
    opacity: 0.5;
    margin-top: 2px;
  }
  .update-banner {
    display: block;
    margin-top: 6px;
    padding: 4px 8px;
    font-size: 0.72rem;
    background-color: color-mix(in srgb, var(--primary-color) 15%, transparent);
    color: var(--primary-color);
    border: 1px solid var(--primary-color);
    border-radius: 4px;
    cursor: pointer;
    text-align: left;
    width: 100%;
    transition: background-color 0.15s ease;
  }
  .update-banner:hover:not(:disabled) {
    background-color: color-mix(in srgb, var(--primary-color) 25%, transparent);
  }
  .update-banner:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }
</style>
