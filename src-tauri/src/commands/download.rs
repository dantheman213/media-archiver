use crate::commands::binaries::check_binaries;
use crate::process_manager::{ProcessEvent, ProcessManager};
use chrono::Local;
use serde::Deserialize;
use std::path::{Path, PathBuf};
use tauri::{AppHandle, Emitter};

/// Sanitize a user-supplied file name so it is safe on disk and inside a yt-dlp
/// output template. Returns a base name without extension; empty string means the
/// caller should fall back to the default `%(title)s`.
/// Keep in sync with src/lib/sanitizeFilename.ts.
fn sanitize_filename(name: &str) -> String {
    // Replace illegal chars, control chars, and '%' (reserved by yt-dlp templates).
    let mut safe: String = name
        .chars()
        .map(|c| match c {
            '<' | '>' | ':' | '"' | '/' | '\\' | '|' | '?' | '*' | '%' => '_',
            c if (c as u32) < 0x20 => '_',
            c => c,
        })
        .collect();

    // Trim surrounding whitespace, then trailing dots/spaces.
    safe = safe.trim().trim_end_matches(['.', ' ']).to_string();

    if safe.is_empty() {
        return String::new();
    }

    // Guard Windows reserved device names (case-insensitive, ignoring extension).
    const RESERVED: [&str; 22] = [
        "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8",
        "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
    ];
    let base = safe.split('.').next().unwrap_or("").to_uppercase();
    if RESERVED.contains(&base.as_str()) {
        safe.push('_');
    }

    safe
}

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct DownloadConfig {
    pub job_id: String,
    pub url: String,
    pub workflow: String,
    pub output_path: String,
    pub output_filename: Option<String>,
    // Optional overrides
    pub target_format: Option<String>,
    pub video_quality: Option<String>,
    pub audio_format: Option<String>,
    pub audio_quality: Option<String>,
    pub embed_subtitles: Option<bool>,
    pub embed_metadata: Option<bool>,
    pub embed_thumbnail: Option<bool>,
    pub trim_start: Option<String>,
    pub trim_end: Option<String>,
    pub use_impersonate_chrome: Option<bool>,
    pub use_no_cookies: Option<bool>,
    pub collect_logs: Option<bool>,
    /// Force a fresh download even if the destination file already exists.
    /// Set by the Retry / Redownload actions.
    pub force: Option<bool>,
}

/// Per-job staging directory for yt-dlp's intermediate and partial files.
///
/// Everything yt-dlp downloads (including `.part` files and the streams that
/// get merged) is written here first and only moved into the user's target
/// folder once the download and any post-processing succeed. A failed job can
/// therefore be cleaned up without leaving stray assets behind.
fn staging_dir_for(job_id: &str) -> PathBuf {
    std::env::temp_dir()
        .join("media-archiver-staging")
        .join(job_id)
}

/// Best-effort removal of a job's staging directory (and the now-empty parent).
async fn cleanup_staging(dir: &Path) {
    let _ = tokio::fs::remove_dir_all(dir).await;
    if let Some(parent) = dir.parent() {
        // Fails harmlessly when other jobs are still staging.
        let _ = tokio::fs::remove_dir(parent).await;
    }
}

/// Flags shared by the primary and fallback yt-dlp invocations.
fn push_common_args(
    args: &mut Vec<String>,
    output_base: &str,
    staging: &Path,
    name_template: &str,
    config: &DownloadConfig,
    ffmpeg_path: &Option<String>,
    collect_logs: bool,
) {
    // Intermediate files are staged here and moved to `home` on success.
    // NOTE: `--paths` is ignored when `--output` is absolute, so the output
    // template below is deliberately relative.
    args.push("-P".to_string());
    args.push(format!("home:{}", output_base));
    args.push("-P".to_string());
    args.push(format!("temp:{}", staging.to_string_lossy()));

    args.push("-o".to_string());
    args.push(format!("{}.%(ext)s", name_template));

    // Restrict filenames to characters valid on Windows
    #[cfg(target_os = "windows")]
    {
        args.push("--windows-filenames".to_string());
    }

    // Progress output for parsing
    args.push("--newline".to_string());
    args.push("--progress".to_string());

    // Print the final file path after all post-processing/moves
    args.push("--print".to_string());
    args.push("after_move:filepath".to_string());

    // When technical logging is enabled, run yt-dlp verbosely. This is the only
    // way to see the exact postprocessor/ffmpeg command line and ffmpeg's own
    // stderr — the raw cause behind generic failures such as
    // "Postprocessing: Error opening output files: Invalid argument".
    if collect_logs {
        args.push("--verbose".to_string());
    }

    // Set ffmpeg location if available. Pass the *directory* containing the
    // binary (not the exe path) so yt-dlp also discovers ffprobe sitting next
    // to it — several post-processors (thumbnail embed, format probing) need
    // ffprobe, and a missing ffprobe is a common cause of hand-off errors.
    if let Some(ffmpeg) = ffmpeg_path {
        let ffmpeg_location = Path::new(ffmpeg)
            .parent()
            .map(|p| p.to_string_lossy().to_string())
            .filter(|s| !s.is_empty())
            .unwrap_or_else(|| ffmpeg.clone());
        args.push("--ffmpeg-location".to_string());
        args.push(ffmpeg_location);
    }

    // Redownload actions overwrite whatever is already on disk.
    if config.force.unwrap_or(false) {
        args.push("--force-overwrites".to_string());
    }

    // Optional yt-dlp flags from settings
    if config.use_impersonate_chrome.unwrap_or(true) {
        args.push("--impersonate".to_string());
        args.push("chrome".to_string());
    }
    if config.use_no_cookies.unwrap_or(true) {
        args.push("--no-cookies".to_string());
    }
}

/// The full-quality invocation: selection + merging/conversion/embedding.
fn push_primary_args(args: &mut Vec<String>, config: &DownloadConfig) {
    // Extra ffmpeg args, keyed to the postprocessor they belong to. An unkeyed
    // `--postprocessor-args` value is handed to *every* postprocessor, which
    // can break thumbnail/metadata embedding; always name the target PP (e.g.
    // "VideoConvertor:-crf 18"). Keyed options may safely be repeated.
    let mut pp_args: Vec<String> = Vec::new();

    match config.workflow.as_str() {
        "audio_only" => {
            args.push("-x".to_string());
            if let Some(ref fmt) = config.audio_format {
                args.push("--audio-format".to_string());
                args.push(fmt.clone());
            }
            if let Some(ref quality) = config.audio_quality {
                let aq = match quality.as_str() {
                    "best" => "0",
                    "balanced" => "5",
                    "small_size" => "9",
                    _ => "5",
                };
                args.push("--audio-quality".to_string());
                args.push(aq.to_string());
            }
        }
        _ => {
            // video_best or custom
            args.push("-f".to_string());
            args.push("bestvideo+bestaudio/best".to_string());

            if let Some(ref fmt) = config.target_format {
                args.push("--merge-output-format".to_string());
                args.push(fmt.clone());
            }

            if let Some(ref quality) = config.video_quality {
                // Map quality preset to postprocessor CRF (applied when a
                // re-encode occurs; scoped to VideoConvertor below).
                let crf = match quality.as_str() {
                    "best" => "18",
                    "balanced" => "23",
                    "small_size" => "28",
                    _ => "23",
                };
                pp_args.push(format!("VideoConvertor:-crf {}", crf));
            }
        }
    }

    // Embed options
    if config.embed_subtitles.unwrap_or(false) {
        args.push("--embed-subs".to_string());
        args.push("--sub-langs".to_string());
        args.push("all".to_string());
    }
    if config.embed_metadata.unwrap_or(true) {
        args.push("--embed-metadata".to_string());
    }
    if config.embed_thumbnail.unwrap_or(true) {
        args.push("--embed-thumbnail".to_string());
    }

    // Trim — use yt-dlp's native section download rather than ffmpeg copy-trim.
    // This is accurate (with --force-keyframes-at-cuts) and only downloads the
    // selected range. Format: --download-sections "*START-END" where either
    // bound may be omitted (start defaults to 0, end to the end of media).
    let trim_start = config
        .trim_start
        .as_deref()
        .map(str::trim)
        .filter(|s| !s.is_empty());
    let trim_end = config
        .trim_end
        .as_deref()
        .map(str::trim)
        .filter(|s| !s.is_empty());
    if trim_start.is_some() || trim_end.is_some() {
        let start = trim_start.unwrap_or("0");
        let end = trim_end.unwrap_or("inf");
        args.push("--download-sections".to_string());
        args.push(format!("*{}-{}", start, end));
        args.push("--force-keyframes-at-cuts".to_string());
    }

    // Emit each collected ffmpeg post-processor arg as its own keyed option.
    for pp in &pp_args {
        args.push("--postprocessor-args".to_string());
        args.push(pp.clone());
    }
}

/// The dependency-free fallback: request a format that needs no ffmpeg, and
/// skip merging/transcoding/embedding entirely. Whatever yt-dlp can fetch as a
/// single file is kept rather than failing the whole job.
fn push_fallback_args(args: &mut Vec<String>, config: &DownloadConfig) {
    match config.workflow.as_str() {
        "audio_only" => {
            // Best audio stream as-is; dropping `-x` avoids an ffmpeg
            // extraction pass.
            args.push("-f".to_string());
            args.push("bestaudio/best".to_string());
        }
        _ => {
            // Prefer a progressive file that already carries both streams so
            // no merge is required.
            args.push("-f".to_string());
            args.push("best[acodec!=none][vcodec!=none]/best".to_string());
        }
    }
}

/// Heuristic: does this stderr tail point at a broken optional dependency
/// (ffmpeg/ffprobe/AtomicParsley/mutagen) rather than a problem with the URL?
fn looks_like_postprocess_failure(tail: &str) -> bool {
    let t = tail.to_lowercase();
    const NEEDLES: [&str; 12] = [
        "postprocessing",
        "post-processing",
        "ffmpeg",
        "ffprobe",
        "unable to merge",
        "merging",
        "conversion failed",
        "error opening output files",
        "invalid argument",
        "atomicparsley",
        "mutagen",
        "could not find ffmpeg",
    ];
    NEEDLES.iter().any(|n| t.contains(n))
}

struct AttemptResult {
    success: bool,
    code: Option<i32>,
    stderr_tail: String,
    process_error: Option<String>,
}

/// Run one yt-dlp attempt to completion, streaming its output to the UI.
async fn run_attempt(
    manager: &ProcessManager,
    yt_dlp_path: &str,
    args: Vec<String>,
    job_id: &str,
    source_label: &str,
    collect_logs: bool,
) -> AttemptResult {
    let mut cmd = tokio::process::Command::new(yt_dlp_path);
    cmd.args(&args);
    crate::process_env::apply_child_env_async(&mut cmd);

    #[cfg(target_os = "windows")]
    {
        cmd.creation_flags(0x08000000);
    }

    let (mut child, stderr_tail, logger) = match manager
        .spawn(
            job_id.to_string(),
            cmd,
            Some(source_label.to_string()),
            collect_logs,
        )
        .await
    {
        Ok(v) => v,
        Err(e) => {
            return AttemptResult {
                success: false,
                code: None,
                stderr_tail: String::new(),
                process_error: Some(e),
            };
        }
    };

    let status = match child.wait().await {
        Ok(s) => s,
        Err(e) => {
            return AttemptResult {
                success: false,
                code: None,
                stderr_tail: String::new(),
                process_error: Some(format!("Process error: {}", e)),
            };
        }
    };

    // Let the stderr reader task drain the final lines before we snapshot them.
    tokio::time::sleep(std::time::Duration::from_millis(80)).await;

    let tail = stderr_tail
        .lock()
        .ok()
        .map(|b| {
            b.iter()
                .rev()
                .take(20)
                .rev()
                .cloned()
                .collect::<Vec<_>>()
                .join("\n")
        })
        .unwrap_or_default();

    if let Some(ref logger) = logger {
        logger
            .note(&format!(
                "----- finished: {} -----",
                Local::now().format("%Y-%m-%d %H:%M:%S")
            ))
            .await;
        logger
            .note(&format!(
                "result: {} (exit code {})",
                if status.success() {
                    "success"
                } else {
                    "failure"
                },
                status.code().unwrap_or(-1)
            ))
            .await;
    }

    AttemptResult {
        success: status.success(),
        code: status.code(),
        stderr_tail: tail,
        process_error: None,
    }
}

#[tauri::command]
pub async fn start_download(app: AppHandle, config: DownloadConfig) -> Result<(), String> {
    let status = check_binaries(app.clone());

    let yt_dlp_path = status
        .yt_dlp_path
        .ok_or_else(|| "yt-dlp not found. Please install or configure it first.".to_string())?;

    let ffmpeg_path = status.ffmpeg_path;
    let collect_logs = config.collect_logs.unwrap_or(true);

    let output_path = if config.output_path.is_empty() {
        dirs_next::download_dir()
            .map(|p| p.to_string_lossy().to_string())
            .ok_or_else(|| {
                "Could not determine a default download path. Please set one in settings."
                    .to_string()
            })?
    } else {
        config.output_path.clone()
    };

    // Output template. Use a sanitized custom file name when provided, else fall
    // back to yt-dlp's title template (which yt-dlp also sanitizes).
    let output_base = output_path.trim_end_matches(['/', '\\']).to_string();
    let name_template = config
        .output_filename
        .as_deref()
        .map(sanitize_filename)
        .filter(|s| !s.is_empty())
        .unwrap_or_else(|| "%(title)s".to_string());

    let staging = staging_dir_for(&config.job_id);

    // Primary attempt: full quality + post-processing.
    let mut primary_args: Vec<String> = Vec::new();
    push_common_args(
        &mut primary_args,
        &output_base,
        &staging,
        &name_template,
        &config,
        &ffmpeg_path,
        collect_logs,
    );
    push_primary_args(&mut primary_args, &config);
    primary_args.push(config.url.clone());

    // Fallback attempt: no ffmpeg-backed post-processing.
    let mut fallback_args: Vec<String> = Vec::new();
    push_common_args(
        &mut fallback_args,
        &output_base,
        &staging,
        &name_template,
        &config,
        &ffmpeg_path,
        collect_logs,
    );
    push_fallback_args(&mut fallback_args, &config);
    fallback_args.push(config.url.clone());

    let job_id = config.job_id.clone();
    let source_label = config.url.clone();
    let manager = ProcessManager::new(app.clone());
    let app_clone = app.clone();

    tokio::spawn(async move {
        let primary = run_attempt(
            &manager,
            &yt_dlp_path,
            primary_args,
            &job_id,
            &source_label,
            collect_logs,
        )
        .await;

        if primary.success {
            let _ = app_clone.emit(
                &format!("process-event-{}", job_id),
                ProcessEvent {
                    job_id: job_id.clone(),
                    event_type: "exit".to_string(),
                    payload: "0".to_string(),
                },
            );
            cleanup_staging(&staging).await;
            return;
        }

        // Retry without post-processing when the failure looks like a broken
        // optional dependency, or when ffmpeg is missing entirely. URL/network
        // failures are left alone so we don't re-download worthless jobs.
        let ffmpeg_missing = ffmpeg_path.is_none();
        let should_fallback = primary.process_error.is_none()
            && (ffmpeg_missing || looks_like_postprocess_failure(&primary.stderr_tail));

        let fallback = if should_fallback {
            let _ = app_clone.emit(
                &format!("process-event-{}", job_id),
                ProcessEvent {
                    job_id: job_id.clone(),
                    event_type: "stdout".to_string(),
                    payload:
                        "[media-archiver] Post-processing failed; retrying with a simpler format (no ffmpeg)."
                            .to_string(),
                },
            );
            Some(
                run_attempt(
                    &manager,
                    &yt_dlp_path,
                    fallback_args,
                    &job_id,
                    &source_label,
                    collect_logs,
                )
                .await,
            )
        } else {
            None
        };

        // Nothing usable was produced — drop the staging assets so the target
        // directory is left exactly as it was.
        cleanup_staging(&staging).await;

        let had_fallback = fallback.is_some();
        let result = fallback.unwrap_or(primary);

        if result.success {
            let _ = app_clone.emit(
                &format!("process-event-{}", job_id),
                ProcessEvent {
                    job_id: job_id.clone(),
                    event_type: "exit".to_string(),
                    payload: "0".to_string(),
                },
            );
        } else {
            let code = result.code.unwrap_or(-1);
            let mut message = if let Some(err) = &result.process_error {
                err.clone()
            } else if result.stderr_tail.trim().is_empty() {
                format!("Download failed with exit code {}", code)
            } else {
                format!(
                    "Download failed (exit code {}):\n{}",
                    code, result.stderr_tail
                )
            };
            if had_fallback {
                message.push_str(
                    "\n\nFallback download without ffmpeg also failed; downloaded assets were cleaned up.",
                );
            }
            let _ = app_clone.emit(
                &format!("process-event-{}", job_id),
                ProcessEvent {
                    job_id: job_id.clone(),
                    event_type: "error".to_string(),
                    payload: message,
                },
            );
        }
    });

    Ok(())
}
