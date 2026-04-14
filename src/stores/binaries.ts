import { writable, get } from 'svelte/store';
import { invoke } from "@tauri-apps/api/core";
import { listen } from "@tauri-apps/api/event";

export interface BinaryStatus {
  yt_dlp_found: boolean;
  ffmpeg_found: boolean;
  atomic_parsley_found: boolean;
  yt_dlp_path?: string;
  ffmpeg_path?: string;
  atomic_parsley_path?: string;
  yt_dlp_version?: string;
  ffmpeg_version?: string;
  atomic_parsley_version?: string;
}

export type BinaryCheckState = 'checking' | 'prompt' | 'installing' | 'manual' | 'done';

export const binaryStatus = writable<BinaryStatus | null>(null);
export const binaryCheckState = writable<BinaryCheckState>('checking');
export const binaryErrorMsg = writable<string>('');
export const binaryInstallProgress = writable<Record<string, number>>({ "yt-dlp": 0, ffmpeg: 0, "ffmpeg-extract": 0, atomicparsley: 0 });

let unlistenProgress: (() => void) | null = null;

export async function checkBinaries() {
  binaryCheckState.set('checking');
  binaryErrorMsg.set('');
  try {
    const res = await invoke<BinaryStatus>("check_binaries");
    binaryStatus.set(res);
    if (res.yt_dlp_found && res.ffmpeg_found && res.atomic_parsley_found) {
      binaryCheckState.set('done');
    } else {
      binaryCheckState.set('prompt');
    }
  } catch (e) {
    binaryErrorMsg.set(String(e));
    binaryCheckState.set('prompt');
  }
}

export async function autoInstallBinaries() {
  binaryCheckState.set('installing');
  binaryErrorMsg.set('');
  
  if (!unlistenProgress) {
    unlistenProgress = await listen("download-progress", (event: any) => {
      const { component, progress } = event.payload;
      binaryInstallProgress.update(p => ({ ...p, [component]: progress }));
    });
  }

  try {
    await invoke("install_binaries");
    await checkBinaries();
  } catch (e) {
    binaryErrorMsg.set(String(e));
    binaryCheckState.set('prompt');
  } finally {
    if (unlistenProgress) {
      unlistenProgress();
      unlistenProgress = null;
    }
  }
}

export interface YtDlpUpdateInfo {
  currentVersion: string;
  latestVersion: string;
  updateAvailable: boolean;
}

export const ytdlpUpdateInfo = writable<YtDlpUpdateInfo | null>(null);
export const ytdlpUpdating = writable<boolean>(false);

export async function checkYtDlpUpdate() {
  try {
    const result = await invoke<YtDlpUpdateInfo | null>('check_ytdlp_update');
    ytdlpUpdateInfo.set(result);
  } catch {
    // Silently fail — update check is not critical
  }
}

export async function performYtDlpUpdate() {
  ytdlpUpdating.set(true);
  try {
    await invoke('update_ytdlp');
    ytdlpUpdateInfo.set(null);
    await checkBinaries();
  } catch (e) {
    console.error('yt-dlp update failed:', e);
  } finally {
    ytdlpUpdating.set(false);
  }
}

export async function saveManualBinaries(customYtDlp: string, customFfmpeg: string) {
  binaryErrorMsg.set('');
  try {
    await invoke("set_binary_paths", {
      ytDlpPath: customYtDlp || null,
      ffmpegPath: customFfmpeg || null,
      atomicParsleyPath: null,
    });
    await checkBinaries();
    if (get(binaryCheckState) !== "done") {
      binaryErrorMsg.set("Binaries not found at provided paths.");
    }
  } catch (e) {
    binaryErrorMsg.set(String(e));
  }
}
