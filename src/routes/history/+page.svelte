<script lang="ts">
  import { completedJobs, clearCompleted } from '../../stores/queue';

  let completed: import('../../types').MediaJob[] = $state([]);

  $effect(() => {
    const unsub = completedJobs.subscribe((j) => (completed = j));
    return unsub;
  });
</script>

<div class="view">
  <header class="view-header">
    <h2>History</h2>
    {#if completed.length > 0}
      <button class="btn-secondary" onclick={() => clearCompleted()}>Clear All</button>
    {/if}
  </header>

  {#if completed.length === 0}
    <div class="empty-state">
      <p>No completed downloads yet.</p>
      <p class="hint">Completed jobs will appear here.</p>
    </div>
  {:else}
    <ul class="job-list">
      {#each completed as job (job.id)}
        <li class="job-item completed">
          <span class="job-title">{job.metadata?.title ?? job.url}</span>
          <span class="job-status">Completed</span>
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

  .job-list {
    list-style: none;
    display: flex;
    flex-direction: column;
    gap: var(--spacing-sm);
  }

  .job-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: var(--spacing-md);
    background-color: var(--bg-surface);
    border: 1px solid var(--border-color);
    border-radius: 8px;
  }

  .job-title {
    font-weight: 500;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    flex: 1;
    margin-right: var(--spacing-md);
  }

  .job-status {
    font-size: 0.85rem;
    color: var(--success-color);
    font-weight: 600;
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
