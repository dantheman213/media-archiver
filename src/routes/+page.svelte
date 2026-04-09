<script lang="ts">
  import { invoke } from "@tauri-apps/api/core";

  let greeting = $state("");
  let name = $state("");

  async function greet() {
    if (name) {
      greeting = await invoke("greet", { name });
    }
  }
</script>

<main class="container">
  <h1>Welcome to Media Archiver</h1>
  
  <p>Tauri, SvelteKit, and Rust are running.</p>

  <div class="row">
    <input id="greet-input" placeholder="Enter a name..." bind:value={name} />
    <button onclick={greet}>Greet</button>
  </div>

  <p>{greeting}</p>
</main>

<style>
  .row {
    display: flex;
    justify-content: flex-start;
    gap: var(--spacing-sm);
    margin-top: var(--spacing-md);
    margin-bottom: var(--spacing-md);
  }

  input {
    padding: var(--spacing-sm);
    border: 1px solid var(--border-color);
    border-radius: 4px;
    font-size: var(--font-size-base);
  }

  button {
    padding: var(--spacing-sm) var(--spacing-md);
    background-color: var(--primary-color);
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: var(--font-size-base);
  }

  button:hover {
    background-color: #0056b3;
  }
</style>