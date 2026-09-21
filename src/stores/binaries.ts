import { writable, get } from 'svelte/store';
import { invoke } from "@tauri-apps/api/core";
import { listen } from "@tauri-apps/api/event";

export interface BinaryStatus {
  yt_dlp_found: boolean;
  ffmpeg_found: boolean;
  ffprobe_found: boolean;
  atomic_parsley_found: boolean;
  yt_dlp_path?: string;
  ffmpeg_path?: string;
  ffprobe_path?: string;
  atomic_parsley_path?: string;
  yt_dlp_version?: string;
  ffmpeg_version?: string;
  ffprobe_version?: string;
  atomic_parsley_version?: string;
}

/** A dependency's download source, surfaced in the Getting Started screen. */
export interface BinarySource {
  name: string;
  url?: string | null;
  /** Explanation used when there is no direct URL (package manager, bundled). */
  note?: string | null;
}

/** User-configured overrides; an empty/missing entry means "auto-detect". */
export interface BinaryPaths {
  yt_dlp_path?: string | null;
  ffmpeg_path?: string | null;
  ffprobe_path?: string | null;
  atomic_parsley_path?: string | null;
}

export type BinaryCheckState = 'checking' | 'prompt' | 'installing' | 'manual' | 'done';

export const binaryStatus = writable<BinaryStatus | null>(null);
export const binarySources = writable<BinarySource[]>([]);
export const binaryCheckState = writable<BinaryCheckState>('checking');
export const binaryErrorMsg = writable<string>('');
export const binaryInstallProgress = writable<Record<string, number>>({ "yt-dlp": 0, ffmpeg: 0, "ffmpeg-extract": 0, atomicparsley: 0 });

let unlistenProgress: (() => void) | null = null;

// Re-entry guard: a second invoke while the first is still running would race
// the same download/extract paths and corrupt the resulting archives.
let installInFlight = false;

/**
 * Detect installed binaries.
 *
 * `silent` is used when re-checking from Settings: it never downgrades an
 * already set-up app back into the first-run overlay, so a user configuring a
 * custom path can't be locked out of the screen they're editing.
 */
export async function checkBinaries(options: { silent?: boolean } = {}) {
  const previous = get(binaryCheckState);
  const wasReady = previous === 'done';
  if (!options.silent) {
    binaryCheckState.set('checking');
  }
  binaryErrorMsg.set('');
  try {
    const res = await invoke<BinaryStatus>("check_binaries");
    binaryStatus.set(res);
    const allFound =
      res.yt_dlp_found && res.ffmpeg_found && res.ffprobe_found && res.atomic_parsley_found;
    if (allFound || (options.silent && wasReady)) {
      binaryCheckState.set('done');
    } else {
      binaryCheckState.set('prompt');
    }
  } catch (e) {
    binaryErrorMsg.set(String(e));
    if (!options.silent) binaryCheckState.set('prompt');
  }
}

/** Load the per-platform list of download sources shown before auto-setup. */
export async function loadBinarySources() {
  try {
    const res = await invoke<BinarySource[]>('get_binary_sources');
    binarySources.set(res);
  } catch (e) {
    console.error('Failed to load binary sources:', e);
  }
}

/** Read the user's configured path overrides (nulls mean auto-detect). */
export async function loadBinaryPaths(): Promise<BinaryPaths> {
  return await invoke<BinaryPaths>('get_binary_paths');
}

/** Persist path overrides and refresh the detected status. */
export async function saveBinaryPaths(paths: BinaryPaths) {
  binaryErrorMsg.set('');
  await invoke('set_binary_paths', {
    ytDlpPath: paths.yt_dlp_path || null,
    ffmpegPath: paths.ffmpeg_path || null,
    ffprobePath: paths.ffprobe_path || null,
    atomicParsleyPath: paths.atomic_parsley_path || null,
  });
  await checkBinaries({ silent: true });
}

export async function autoInstallBinaries() {
  if (installInFlight) return;
  installInFlight = true;

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
    installInFlight = false;
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
export const ytdlpChecking = writable<boolean>(false);
export const ytdlpUpdateError = writable<string>('');
/** Result of the last "Update yt-dlp" run (success text or failure reason). */
export const ytdlpUpdateMessage = writable<string>('');

export async function checkYtDlpUpdate() {
  ytdlpChecking.set(true);
  ytdlpUpdateError.set('');
  try {
    const result = await invoke<YtDlpUpdateInfo | null>('check_ytdlp_update');
    ytdlpUpdateInfo.set(result);
  } catch (e) {
    // Surface the failure (network / GitHub rate limit) rather than hiding it —
    // a swallowed error is why the check previously appeared to do nothing.
    ytdlpUpdateError.set(String(e));
  } finally {
    ytdlpChecking.set(false);
  }
}

export async function performYtDlpUpdate() {
  ytdlpUpdating.set(true);
  ytdlpUpdateMessage.set('');
  try {
    const msg: string = await invoke('update_ytdlp');
    ytdlpUpdateMessage.set(msg);
    ytdlpUpdateInfo.set(null);
    await checkBinaries();
  } catch (e) {
    ytdlpUpdateMessage.set(`Update failed: ${String(e)}`);
  } finally {
    ytdlpUpdating.set(false);
  }
}

export async function saveManualBinaries(customYtDlp: string, customFfmpeg: string) {
  binaryErrorMsg.set('');
  try {
    await saveBinaryPaths({
      yt_dlp_path: customYtDlp,
      ffmpeg_path: customFfmpeg,
    });
    if (get(binaryCheckState) !== "done") {
      binaryErrorMsg.set("Binaries not found at provided paths.");
    }
  } catch (e) {
    binaryErrorMsg.set(String(e));
  }
}
