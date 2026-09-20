use std::path::PathBuf;
use std::time::{Duration, SystemTime};
use tauri::{AppHandle, Manager};
use tokio::fs;

/// Retention used when the persisted setting is missing.
pub const DEFAULT_LOG_RETENTION_HOURS: u32 = 720;

/// Directory holding one technical log per video ingestion.
pub fn logs_dir(app: &AppHandle) -> PathBuf {
    let mut path = app
        .path()
        .app_local_data_dir()
        .unwrap_or_else(|_| PathBuf::from("."));
    path.push("logs");
    path
}

/// Open the logs folder in the OS file manager, creating it if needed.
#[tauri::command]
pub async fn open_logs_folder(app: AppHandle) -> Result<(), String> {
    let dir = logs_dir(&app);
    fs::create_dir_all(&dir)
        .await
        .map_err(|e| format!("Failed to create logs directory: {}", e))?;
    crate::commands::file_ops::open_path_in_os(&dir.to_string_lossy())
}

/// Delete every technical log.
#[tauri::command]
pub async fn clear_logs(app: AppHandle) -> Result<(), String> {
    let dir = logs_dir(&app);
    if dir.exists() {
        fs::remove_dir_all(&dir)
            .await
            .map_err(|e| format!("Failed to clear logs: {}", e))?;
    }
    Ok(())
}

/// Delete `.log` files whose modification time is older than `retention_hours`.
/// A retention of `0` disables pruning.
pub async fn prune_logs(app: &AppHandle, retention_hours: u32) {
    if retention_hours == 0 {
        return;
    }

    let dir = logs_dir(app);
    if !dir.exists() {
        return;
    }

    let cutoff = SystemTime::now()
        .checked_sub(Duration::from_secs(retention_hours as u64 * 3_600));

    let mut entries = match fs::read_dir(&dir).await {
        Ok(entries) => entries,
        Err(_) => return,
    };

    while let Ok(Some(entry)) = entries.next_entry().await {
        let path = entry.path();
        if path.extension().and_then(|e| e.to_str()) != Some("log") {
            continue;
        }

        let modified = entry
            .metadata()
            .await
            .ok()
            .and_then(|m| m.modified().ok());

        if let (Some(modified), Some(cutoff)) = (modified, cutoff) {
            if modified < cutoff {
                let _ = fs::remove_file(&path).await;
            }
        }
    }
}

/// Read the persisted settings and prune old logs. Intended to run once when
/// the app opens, so the rotation period is only evaluated at launch.
pub async fn run_startup_log_rotation(app: &AppHandle) {
    let mut path = match app.path().app_local_data_dir() {
        Ok(p) => p,
        Err(_) => return,
    };
    path.push("settings.json");

    let content = match fs::read_to_string(&path).await {
        Ok(c) => c,
        Err(_) => return,
    };

    let value: serde_json::Value = match serde_json::from_str(&content) {
        Ok(v) => v,
        Err(_) => return,
    };

    // Prefer the hours-based setting; fall back to the legacy days-based one.
    let hours = value
        .get("logRetentionHours")
        .and_then(|v| v.as_u64())
        .or_else(|| {
            value
                .get("logRetentionDays")
                .and_then(|v| v.as_u64())
                .map(|days| days * 24)
        })
        .unwrap_or(DEFAULT_LOG_RETENTION_HOURS as u64) as u32;

    prune_logs(app, hours).await;
}
