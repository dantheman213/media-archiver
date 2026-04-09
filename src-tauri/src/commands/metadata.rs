use crate::commands::binaries::check_binaries;
use crate::models::media::MediaMetadata;
use tauri::AppHandle;

#[tauri::command]
pub async fn fetch_metadata(app: AppHandle, url: String) -> Result<MediaMetadata, String> {
    let status = check_binaries(app.clone());
    let yt_dlp_path = status
        .yt_dlp_path
        .ok_or_else(|| "yt-dlp not found".to_string())?;

    let mut cmd = tokio::process::Command::new(yt_dlp_path);
    cmd.arg("--dump-json").arg(&url);

    #[cfg(target_os = "windows")]
    {
        cmd.creation_flags(0x08000000);
    }

    let output = cmd
        .output()
        .await
        .map_err(|e| format!("Failed to execute yt-dlp: {}", e))?;

    if !output.status.success() {
        let err_msg = String::from_utf8_lossy(&output.stderr);
        return Err(format!("yt-dlp error: {}", err_msg));
    }

    let json_str = String::from_utf8_lossy(&output.stdout);
    let first_line = json_str.lines().next().unwrap_or("{}");

    let metadata: MediaMetadata =
        serde_json::from_str(first_line).map_err(|e| format!("Failed to parse metadata: {}", e))?;

    Ok(metadata)
}
