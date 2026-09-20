// Thumbnail embedding safety.
//
// yt-dlp embeds a thumbnail by first converting it to PNG/JPEG when the
// container needs it, and that conversion forces ffmpeg's `image2` demuxer.
// `image2` cannot probe AVIF (and similar container-based formats), so the
// conversion fails with "Error opening output files: Invalid argument" and
// takes the whole download down with it. Skip `--embed-thumbnail` for those
// formats instead of failing the download.

const EMBEDDABLE_THUMBNAIL_EXTS = new Set(['jpg', 'jpeg', 'png', 'webp', 'gif']);

/** Extract a lowercased file extension from a URL, ignoring query/hash. */
function urlExtension(url: string): string {
  const path = url.split(/[?#]/)[0];
  const lastSegment = path.slice(path.lastIndexOf('/') + 1);
  const dot = lastSegment.lastIndexOf('.');
  return dot <= 0 ? '' : lastSegment.slice(dot + 1).toLowerCase();
}

/**
 * Whether a thumbnail can be safely embedded by yt-dlp. URLs without a
 * recognizable extension are treated as embeddable to preserve the previous
 * default behavior.
 */
export function isThumbnailEmbeddable(thumbnailUrl: string | null | undefined): boolean {
  if (!thumbnailUrl) return true;
  const ext = urlExtension(thumbnailUrl);
  if (!ext) return true;
  return EMBEDDABLE_THUMBNAIL_EXTS.has(ext);
}
