use crate::commands::binaries::check_binaries;
use crate::process_manager::{ProcessEvent, ProcessManager};
use serde::Deserialize;
use tauri::{AppHandle, Emitter};

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct DownloadConfig {
    pub job_id: String,
    pub url: String,
    pub workflow: String,
    pub output_path: String,
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
}

#[tauri::command]
pub async fn start_download(app: AppHandle, config: DownloadConfig) -> Result<(), String> {
    let status = check_binaries(app.clone());

    let yt_dlp_path = status
        .yt_dlp_path
        .ok_or_else(|| "yt-dlp not found. Please install or configure it first.".to_string())?;

    let ffmpeg_path = status.ffmpeg_path;

    let mut args: Vec<String> = Vec::new();

    let output_path = if config.output_path.is_empty() {
        dirs_next::download_dir()
            .map(|p| p.to_string_lossy().to_string())
            .ok_or_else(|| "Could not determine a default download path. Please set one in settings.".to_string())?
    } else {
        config.output_path.clone()
    };

    // Output template
    args.push("-o".to_string());
    args.push(format!(
        "{}/%(title)s.%(ext)s",
        output_path.trim_end_matches('/')
    ));

    // Progress output for parsing
    args.push("--newline".to_string());
    args.push("--progress".to_string());

    // Set ffmpeg location if available
    if let Some(ref ffmpeg) = ffmpeg_path {
        args.push("--ffmpeg-location".to_string());
        args.push(ffmpeg.clone());
    }

    // Workflow-specific args
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
                // Map quality preset to postprocessor CRF
                let crf = match quality.as_str() {
                    "best" => "18",
                    "balanced" => "23",
                    "small_size" => "28",
                    _ => "23",
                };
                args.push("--postprocessor-args".to_string());
                args.push(format!("-crf {}", crf));
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

    // Trim
    if let Some(ref start) = config.trim_start {
        if !start.is_empty() {
            args.push("--postprocessor-args".to_string());
            args.push(format!("-ss {}", start));
        }
    }
    if let Some(ref end) = config.trim_end {
        if !end.is_empty() {
            args.push("--postprocessor-args".to_string());
            args.push(format!("-to {}", end));
        }
    }

    // The URL must be last
    args.push(config.url.clone());

    let job_id = config.job_id.clone();
    let manager = ProcessManager::new(app.clone());

    let mut cmd = tokio::process::Command::new(&yt_dlp_path);
    cmd.args(&args);

    #[cfg(target_os = "windows")]
    {
        cmd.creation_flags(0x08000000);
    }

    let mut child = manager
        .spawn(job_id.clone(), cmd)
        .await
        .map_err(|e| format!("Failed to start download: {}", e))?;

    // Wait for process to finish in background
    let app_clone = app.clone();
    let job_id_clone = job_id.clone();
    tokio::spawn(async move {
        match child.wait().await {
            Ok(exit_status) => {
                let event_type = if exit_status.success() {
                    "exit"
                } else {
                    "error"
                };
                let payload = if exit_status.success() {
                    "0".to_string()
                } else {
                    format!(
                        "Download failed with exit code {}",
                        exit_status.code().unwrap_or(-1)
                    )
                };
                let _ = app_clone.emit(
                    &format!("process-event-{}", job_id_clone),
                    ProcessEvent {
                        job_id: job_id_clone,
                        event_type: event_type.to_string(),
                        payload,
                    },
                );
            }
            Err(e) => {
                let _ = app_clone.emit(
                    &format!("process-event-{}", job_id_clone),
                    ProcessEvent {
                        job_id: job_id_clone,
                        event_type: "error".to_string(),
                        payload: format!("Process error: {}", e),
                    },
                );
            }
        }
    });

    Ok(())
}
