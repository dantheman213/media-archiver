<script lang="ts">
  import '../app.css';
  import Onboarding from '../components/Onboarding.svelte';
  import ErrorToast from '../components/ErrorToast.svelte';
  import { page } from '$app/stores';
  import { goto } from '$app/navigation';
  import { toggleGlobalPause, settings, initializeSettings } from '../stores/settings';
  import { selectedJobId, removeJob } from '../stores/queue';
  import { checkBinaries, binaryCheckState } from '../stores/binaries';
  import type { NavRoute } from '../types';
  import { onMount } from 'svelte';

  let { children } = $props();

  interface NavItem {
    route: NavRoute;
    label: string;
    path: string;
  }

  const navItems: NavItem[] = [
    { route: 'queue', label: 'Queue', path: '/' },
    { route: 'history', label: 'History', path: '/history' },
    { route: 'settings', label: 'Settings', path: '/settings' },
    { route: 'binaries', label: 'Binaries Status', path: '/binaries' },
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

  onMount(() => {
    checkBinaries();
    initializeSettings();
  });

  $effect(() => {
    if ($settings.theme === 'light' || $settings.theme === 'dark') {
      document.documentElement.setAttribute('data-theme', $settings.theme);
    } else {
      document.documentElement.removeAttribute('data-theme');
    }
  });

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
        Checking binaries...
      {:else if $settings.globalPaused}
        Paused
      {:else}
        Ready
      {/if}
    </div>
  </aside>
  <main class="main-content">
    {@render children()}
  </main>
</div>
