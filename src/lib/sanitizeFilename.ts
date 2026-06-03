// Sanitize a user-supplied filename so it is safe to use as a real file name on
// disk and inside a yt-dlp output template. Returns the base name WITHOUT an
// extension (yt-dlp appends `.%(ext)s`). Returns '' when nothing usable remains,
// in which case callers should fall back to the default `%(title)s` template.
//
// Keep these rules in sync with `sanitize_filename` in
// src-tauri/src/commands/download.rs.

const WINDOWS_RESERVED = new Set([
  'CON', 'PRN', 'AUX', 'NUL',
  'COM1', 'COM2', 'COM3', 'COM4', 'COM5', 'COM6', 'COM7', 'COM8', 'COM9',
  'LPT1', 'LPT2', 'LPT3', 'LPT4', 'LPT5', 'LPT6', 'LPT7', 'LPT8', 'LPT9',
]);

export function sanitizeFilename(name: string): string {
  // Replace characters illegal in filenames, control chars, and '%' (reserved by
  // yt-dlp's output template) with an underscore.
  // eslint-disable-next-line no-control-regex
  let safe = name.replace(/[<>:"/\\|?*%\x00-\x1F]/g, '_');

  // Trim trailing dots/spaces (illegal/awkward on Windows) and leading/trailing whitespace.
  safe = safe.trim().replace(/[. ]+$/g, '');

  if (!safe) return '';

  // Guard Windows reserved device names (case-insensitive, ignoring any extension part).
  const base = safe.split('.')[0].toUpperCase();
  if (WINDOWS_RESERVED.has(base)) {
    safe = `${safe}_`;
  }

  return safe;
}
