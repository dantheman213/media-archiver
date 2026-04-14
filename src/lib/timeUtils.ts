/** Parse a time string (HH:MM:SS or MM:SS) into total seconds, or null if invalid */
export function parseTimeString(str: string): number | null {
  if (!str) return null;
  const trimmed = str.trim();

  // Match HH:MM:SS or MM:SS
  const match = trimmed.match(/^(?:(\d{1,2}):)?(\d{1,2}):(\d{2})$/);
  if (!match) return null;

  const hours = match[1] ? parseInt(match[1], 10) : 0;
  const minutes = parseInt(match[2], 10);
  const seconds = parseInt(match[3], 10);

  if (minutes >= 60 || seconds >= 60) return null;

  return hours * 3600 + minutes * 60 + seconds;
}

/** Format seconds into HH:MM:SS (or MM:SS if under an hour) */
export function formatTime(totalSeconds: number): string {
  const h = Math.floor(totalSeconds / 3600);
  const m = Math.floor((totalSeconds % 3600) / 60);
  const s = Math.floor(totalSeconds % 60);
  if (h > 0) {
    return `${h}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
  }
  return `${m}:${String(s).padStart(2, '0')}`;
}

/** Check if a string is a valid time format (HH:MM:SS or MM:SS) */
export function isValidTimeString(str: string): boolean {
  if (!str || !str.trim()) return true; // Empty is valid (optional field)
  return parseTimeString(str) !== null;
}
