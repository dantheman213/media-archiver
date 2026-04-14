<script lang="ts">
  let { checked = false, onchange, label, description }: {
    checked: boolean;
    onchange: (checked: boolean) => void;
    label: string;
    description?: string;
  } = $props();

  function handleClick() {
    onchange(!checked);
  }

  function handleKeydown(e: KeyboardEvent) {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      onchange(!checked);
    }
  }
</script>

<div
  class="toggle-switch-row"
  role="switch"
  tabindex="0"
  aria-checked={checked}
  aria-label={label}
  onclick={handleClick}
  onkeydown={handleKeydown}
>
  <div class="toggle-text">
    <span class="toggle-label">{label}</span>
    {#if description}
      <span class="toggle-desc">{description}</span>
    {/if}
  </div>
  <div class="toggle-track" class:active={checked}>
    <div class="toggle-thumb"></div>
  </div>
</div>

<style>
  .toggle-switch-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: var(--spacing-md);
    padding: var(--spacing-xs) 0;
    cursor: pointer;
    user-select: none;
  }

  .toggle-switch-row:focus-visible {
    outline: 2px solid var(--primary-color);
    outline-offset: 2px;
    border-radius: var(--radius-sm);
  }

  .toggle-text {
    display: flex;
    flex-direction: column;
    gap: 1px;
  }

  .toggle-label {
    font-size: var(--font-size-sm);
    font-weight: 500;
  }

  .toggle-desc {
    font-size: 0.75rem;
    color: var(--text-muted);
  }

  .toggle-track {
    position: relative;
    width: 40px;
    height: 22px;
    border-radius: 11px;
    background-color: var(--border-color);
    flex-shrink: 0;
    transition: background-color 0.2s ease;
  }

  .toggle-track.active {
    background-color: var(--primary-color);
  }

  .toggle-thumb {
    position: absolute;
    top: 2px;
    left: 2px;
    width: 18px;
    height: 18px;
    border-radius: 50%;
    background-color: #fff;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
    transition: transform 0.2s ease;
  }

  .toggle-track.active .toggle-thumb {
    transform: translateX(18px);
  }
</style>
