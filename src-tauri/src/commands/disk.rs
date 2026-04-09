use serde::Serialize;
use std::path::Path;

#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
pub struct DiskSpace {
    pub available_bytes: u64,
    pub total_bytes: u64,
}

#[tauri::command]
pub fn get_default_download_dir() -> Option<String> {
    dirs_next::download_dir().map(|p| p.to_string_lossy().to_string())
}

#[tauri::command]
pub fn select_directory() -> Result<Option<String>, String> {
    #[cfg(target_os = "windows")]
    {
        let folder = rfd::FileDialog::new()
            .pick_folder()
            .map(|p| p.to_string_lossy().to_string());
        Ok(folder)
    }
    #[cfg(not(target_os = "windows"))]
    {
        // Stub for now as requested
        Ok(None)
    }
}

#[tauri::command]
pub fn pick_file() -> Result<Option<String>, String> {
    #[cfg(target_os = "windows")]
    {
        let file = rfd::FileDialog::new()
            .pick_file()
            .map(|p| p.to_string_lossy().to_string());
        Ok(file)
    }
    #[cfg(not(target_os = "windows"))]
    {
        // Stub for now as requested
        Ok(None)
    }
}

#[tauri::command]
pub fn get_disk_space(path: String) -> Result<DiskSpace, String> {
    let target = if path.is_empty() {
        // Fall back to the home directory or root
        dirs_next::home_dir()
            .map(|p| p.to_string_lossy().to_string())
            .unwrap_or_else(|| {
                if cfg!(windows) {
                    "C:\\".to_string()
                } else {
                    "/".to_string()
                }
            })
    } else {
        path
    };

    let p = Path::new(&target);
    let metadata = fs4::available_space(p)
        .map_err(|e| format!("Failed to get available space: {}", e))?;
    let total = fs4::total_space(p)
        .map_err(|e| format!("Failed to get total space: {}", e))?;

    Ok(DiskSpace {
        available_bytes: metadata,
        total_bytes: total,
    })
}
