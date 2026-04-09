<script lang="ts">
  import { listen } from '@tauri-apps/api/event';
  import { onMount } from 'svelte';

  interface AppError {
    title: string;
    message: string;
  }

  interface ToastItem {
    id: number;
    title: string;
    message: string;
  }

  let toasts = $state<ToastItem[]>([]);
  let nextId = 0;

  function addToast(error: AppError) {
    const id = nextId++;
    toasts = [...toasts, { id, title: error.title, message: error.message }];
    // Auto-dismiss after 8 seconds
    setTimeout(() => dismiss(id), 8000);
  }

  function dismiss(id: number) {
    toasts = toasts.filter((t) => t.id !== id);
  }

  onMount(() => {
    const unlisten = listen<AppError>('app-error', (event) => {
      addToast(event.payload);
    });
    return () => {
      unlisten.then((fn) => fn());
    };
  });
</script>

{#if toasts.length > 0}
  <div class="toast-container">
    {#each toasts as toast (toast.id)}
      <div class="toast-item">
        <div class="toast-content">
          <strong class="toast-title">{toast.title}</strong>
          <p class="toast-message">{toast.message}</p>
        </div>
        <button class="toast-close" onclick={() => dismiss(toast.id)} aria-label="Dismiss">&times;</button>
      </div>
    {/each}
  </div>
{/if}

<style>
  .toast-container {
    position: fixed;
    bottom: var(--spacing-lg, 16px);
    right: var(--spacing-lg, 16px);
    z-index: 9999;
    display: flex;
    flex-direction: column;
    gap: var(--spacing-sm, 8px);
    max-width: 420px;
  }

  .toast-item {
    display: flex;
    align-items: flex-start;
    gap: var(--spacing-sm, 8px);
    padding: var(--spacing-md, 12px);
    background-color: var(--bg-surface, #1e1e1e);
    border: 1px solid var(--error-color, #dc3545);
    border-radius: var(--radius-md, 8px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
    animation: slide-in 0.2s ease-out;
  }

  @keyframes slide-in {
    from {
      opacity: 0;
      transform: translateX(20px);
    }
    to {
      opacity: 1;
      transform: translateX(0);
    }
  }

  .toast-content {
    flex: 1;
    min-width: 0;
  }

  .toast-title {
    display: block;
    font-size: 0.85rem;
    color: var(--error-color, #dc3545);
    margin-bottom: 2px;
  }

  .toast-message {
    font-size: 0.8rem;
    color: var(--text-muted, #999);
    margin: 0;
    word-break: break-word;
  }

  .toast-close {
    background: none;
    border: none;
    color: var(--text-muted, #999);
    font-size: 1.2rem;
    cursor: pointer;
    padding: 0;
    line-height: 1;
    flex-shrink: 0;
  }

  .toast-close:hover {
    color: var(--text-color, #fff);
  }
</style>
