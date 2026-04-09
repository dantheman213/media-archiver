<script lang="ts">
  import { invoke } from "@tauri-apps/api/core";
  import { listen } from "@tauri-apps/api/event";
  import { open } from "@tauri-apps/plugin-dialog";

  let { onComplete } = $props<{ onComplete: () => void }>();

  let status = $state("checking"); // checking, prompt, installing, manual, done
  let errorMsg = $state("");
  let installProgress: Record<string, number> = $state({ "yt-dlp": 0, ffmpeg: 0, "ffmpeg-extract": 0 });
  
  let customYtDlp = $state("");
  let customFfmpeg = $state("");

  async function checkBinaries() {
    try {
      let res: any = await invoke("check_binaries");
      if (res.yt_dlp_found && res.ffmpeg_found) {
        status = "done";
        onComplete();
      } else {
        status = "prompt";
      }
    } catch (e) {
      errorMsg = String(e);
      status = "prompt";
    }
  }

  async function autoInstall() {
    status = "installing";
    errorMsg = "";
    
    const unlisten = await listen("download-progress", (event: any) => {
      const { component, progress } = event.payload;
      installProgress[component] = progress;
    });

    try {
      await invoke("install_binaries");
      status = "done";
      onComplete();
    } catch (e) {
      errorMsg = String(e);
      status = "prompt";
    } finally {
      unlisten();
    }
  }

  async function saveManual() {
    errorMsg = "";
    try {
      await invoke("set_binary_paths", { 
        ytDlpPath: customYtDlp || null, 
        ffmpegPath: customFfmpeg || null 
      });
      await checkBinaries();
      if (status !== "done") {
        errorMsg = "Binaries not found at provided paths.";
      }
    } catch (e) {
      errorMsg = String(e);
    }
  }
  
  async function selectPath(type: "yt-dlp" | "ffmpeg") {
    const selected = await open({
      multiple: false,
      directory: false,
    });
    if (selected && typeof selected === 'string') {
      if (type === "yt-dlp") {
        customYtDlp = selected;
      } else {
        customFfmpeg = selected;
      }
    }
  }

  // Use a timeout or onMount so it doesn't run during SSR or block initial render
  import { onMount } from "svelte";
  onMount(() => {
    checkBinaries();
  });
</script>

{#if status !== "done"}
<div class="onboarding-overlay">
  <div class="onboarding-modal">
    <h1>Binary Setup</h1>
    <p>Media Archiver needs <code>yt-dlp</code> and <code>ffmpeg</code> to function.</p>

    {#if errorMsg}
      <div class="error">{errorMsg}</div>
    {/if}

    {#if status === "checking"}
      <p>Checking system for binaries...</p>
    {:else if status === "prompt"}
      <div class="options">
        <div class="option-card">
          <h2>Auto-Install (Recommended)</h2>
          <p>We will securely download the required binaries directly from their official sources to your local app data folder.</p>
          <button onclick={autoInstall} class="btn-primary">Download & Install</button>
        </div>
        <div class="option-card">
          <h2>Manual Setup (Advanced)</h2>
          <p>If you already have them installed, you can provide the paths to the executables.</p>
          <button onclick={() => status = "manual"} class="btn-secondary">Setup Manually</button>
        </div>
      </div>
    {:else if status === "installing"}
      <div class="progress-section">
        <h2>Downloading Binaries...</h2>
        
        <div class="progress-item">
          <span>yt-dlp</span>
          <progress value={installProgress["yt-dlp"]} max="1"></progress>
          <span>{Math.round(installProgress["yt-dlp"] * 100)}%</span>
        </div>
        
        <div class="progress-item">
          <span>ffmpeg archive</span>
          <progress value={installProgress["ffmpeg"]} max="1"></progress>
          <span>{Math.round(installProgress["ffmpeg"] * 100)}%</span>
        </div>
        
        {#if installProgress["ffmpeg"] >= 1 && installProgress["ffmpeg-extract"] < 1}
          <div class="progress-item">
            <span>Extracting ffmpeg...</span>
            <progress value={installProgress["ffmpeg-extract"]} max="1"></progress>
          </div>
        {/if}
      </div>
    {:else if status === "manual"}
      <div class="manual-section">
        <h2>Manual Setup</h2>
        
        <div class="input-group">
          <label for="ytdlp-path">yt-dlp Executable Path:</label>
          <div class="path-input">
            <input id="ytdlp-path" type="text" bind:value={customYtDlp} placeholder="e.g. C:\bin\yt-dlp.exe" />
            <button onclick={() => selectPath("yt-dlp")}>Browse</button>
          </div>
        </div>

        <div class="input-group">
          <label for="ffmpeg-path">ffmpeg Executable Path:</label>
          <div class="path-input">
            <input id="ffmpeg-path" type="text" bind:value={customFfmpeg} placeholder="e.g. C:\bin\ffmpeg.exe" />
            <button onclick={() => selectPath("ffmpeg")}>Browse</button>
          </div>
        </div>

        <div class="actions">
          <button onclick={() => status = "prompt"} class="btn-secondary">Back</button>
          <button onclick={saveManual} class="btn-primary">Save & Continue</button>
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
    background-color: var(--bg-color, #1a1a1a);
    color: var(--text-color, #eee);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 9999;
  }

  .onboarding-modal {
    background-color: #2a2a2a;
    padding: 2rem;
    border-radius: 8px;
    max-width: 600px;
    width: 100%;
    box-shadow: 0 4px 12px rgba(0,0,0,0.5);
  }

  .error {
    background-color: #ffcccc;
    color: #cc0000;
    padding: 0.5rem;
    border-radius: 4px;
    margin-bottom: 1rem;
  }

  .options {
    display: flex;
    gap: 1rem;
    margin-top: 1.5rem;
  }

  .option-card {
    flex: 1;
    background-color: #333;
    padding: 1.5rem;
    border-radius: 6px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
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
    background-color: #007bff;
    color: white;
  }

  .btn-secondary {
    background-color: #555;
    color: white;
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
    border: 1px solid #555;
    border-radius: 4px;
    background-color: #222;
    color: white;
  }

  .actions {
    display: flex;
    justify-content: flex-end;
    gap: 1rem;
    margin-top: 2rem;
  }
</style>
