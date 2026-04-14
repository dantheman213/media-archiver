<script lang="ts">
  import { invoke } from "@tauri-apps/api/core";
  import { binaryCheckState, binaryErrorMsg, binaryInstallProgress, autoInstallBinaries, saveManualBinaries } from '../stores/binaries';

  let customYtDlp = $state("");
  let customFfmpeg = $state("");

  async function selectPath(type: "yt-dlp" | "ffmpeg") {
    try {
      const selected = await invoke<string | null>("pick_file");
      if (selected) {
        if (type === "yt-dlp") {
          customYtDlp = selected;
        } else {
          customFfmpeg = selected;
        }
      }
    } catch (e) {
      console.error("Failed to pick file:", e);
    }
  }

  function handleSaveManual() {
    saveManualBinaries(customYtDlp, customFfmpeg);
  }
</script>

{#if $binaryCheckState === "prompt" || $binaryCheckState === "installing" || $binaryCheckState === "manual"}
<div class="onboarding-overlay">
  <div class="onboarding-modal">
    <h1>Getting Started</h1>
    <p>Media Archiver needs a couple of small helper tools to download and process media. We can set them up for you automatically.</p>

    {#if $binaryErrorMsg}
      <div class="error">{$binaryErrorMsg}</div>
    {/if}

    {#if $binaryCheckState === "prompt"}
      <div class="options">
        <div class="option-card">
          <h2>Automatic Setup (Recommended)</h2>
          <p>We'll download everything you need. This takes about a minute.</p>
          <button onclick={autoInstallBinaries} class="btn-primary">Set Up Automatically</button>
        </div>
        <div class="option-card">
          <h2>I Already Have These Tools</h2>
          <p>If you already have the required tools installed, you can point us to them.</p>
          <button onclick={() => $binaryCheckState = "manual"} class="btn-secondary">Set Up Manually</button>
        </div>
      </div>
    {:else if $binaryCheckState === "installing"}
      <div class="progress-section">
        <h2>Setting things up...</h2>

        <div class="progress-item">
          <span>Download engine</span>
          <progress value={$binaryInstallProgress["yt-dlp"]} max="1"></progress>
          <span>{Math.round($binaryInstallProgress["yt-dlp"] * 100)}%</span>
        </div>

        <div class="progress-item">
          <span>Media processor</span>
          <progress value={$binaryInstallProgress["ffmpeg"]} max="1"></progress>
          <span>{Math.round($binaryInstallProgress["ffmpeg"] * 100)}%</span>
        </div>

        {#if $binaryInstallProgress["ffmpeg"] >= 1 && $binaryInstallProgress["ffmpeg-extract"] < 1}
          <div class="progress-item">
            <span>Unpacking media processor...</span>
            <progress value={$binaryInstallProgress["ffmpeg-extract"]} max="1"></progress>
          </div>
        {/if}
      </div>
    {:else if $binaryCheckState === "manual"}
      <div class="manual-section">
        <h2>Manual Setup</h2>

        <div class="input-group">
          <label for="ytdlp-path">Download Engine (yt-dlp):</label>
          <div class="path-input">
            <input id="ytdlp-path" type="text" bind:value={customYtDlp} placeholder="Select the yt-dlp executable..." />
            <button onclick={() => selectPath("yt-dlp")}>Browse</button>
          </div>
        </div>

        <div class="input-group">
          <label for="ffmpeg-path">Media Processor (ffmpeg):</label>
          <div class="path-input">
            <input id="ffmpeg-path" type="text" bind:value={customFfmpeg} placeholder="Select the ffmpeg executable..." />
            <button onclick={() => selectPath("ffmpeg")}>Browse</button>
          </div>
        </div>

        <div class="actions">
          <button onclick={() => $binaryCheckState = "prompt"} class="btn-secondary">Back</button>
          <button onclick={handleSaveManual} class="btn-primary">Save & Continue</button>
        </div>
      </div>
    {/if}
  </div>
</div>
{/if}

<style>
  .onboarding-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.7);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 9999;
  }

  .onboarding-modal {
    background-color: var(--bg-surface);
    color: var(--text-color);
    padding: 2rem;
    border-radius: 8px;
    max-width: 600px;
    width: 100%;
    box-shadow: 0 4px 12px rgba(0,0,0,0.5);
    border: 1px solid var(--border-color);
  }

  .error {
    background-color: rgba(220, 53, 69, 0.1);
    color: var(--error-color);
    padding: 0.5rem;
    border-radius: 4px;
    margin-bottom: 1rem;
    border: 1px solid var(--error-color);
  }

  .options {
    display: flex;
    gap: 1rem;
    margin-top: 1.5rem;
  }

  .option-card {
    flex: 1;
    background-color: var(--bg-surface-hover);
    padding: 1.5rem;
    border-radius: 6px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    border: 1px solid var(--border-color);
  }

  .option-card h2 {
    font-size: 1.2rem;
    margin-top: 0;
  }

  button {
    padding: 0.5rem 1rem;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: bold;
    margin-top: 1rem;
  }

  .btn-primary {
    background-color: var(--primary-color);
    color: white;
  }

  .btn-primary:hover {
    background-color: var(--primary-hover);
  }

  .btn-secondary {
    background-color: var(--secondary-color);
    color: white;
  }

  .btn-secondary:hover {
    opacity: 0.9;
  }

  .progress-section {
    margin-top: 1.5rem;
  }

  .progress-item {
    display: flex;
    align-items: center;
    gap: 1rem;
    margin-bottom: 1rem;
  }

  progress {
    flex: 1;
    height: 1.5rem;
  }

  .manual-section {
    margin-top: 1.5rem;
  }

  .input-group {
    margin-bottom: 1rem;
  }

  .input-group label {
    display: block;
    margin-bottom: 0.5rem;
  }

  .path-input {
    display: flex;
    gap: 0.5rem;
  }

  .path-input input {
    flex: 1;
    padding: 0.5rem;
    border: 1px solid var(--border-color);
    border-radius: 4px;
    background-color: var(--bg-color);
    color: var(--text-color);
  }

  .path-input button {
    margin-top: 0;
  }

  .actions {
    display: flex;
    justify-content: flex-end;
    gap: 1rem;
    margin-top: 2rem;
  }
</style>
