use std::path::PathBuf;
use tauri::{AppHandle, Manager};
use tokio::fs;
use tokio::io::AsyncWriteExt;

fn get_storage_path(app: &AppHandle, key: &str) -> PathBuf {
    let mut path = app
        .path()
        .app_local_data_dir()
        .unwrap_or_else(|_| PathBuf::from("."));
    path.push(format!("{}.json", key));
    path
}

#[tauri::command]
pub async fn save_json(app: AppHandle, key: String, value: String) -> Result<(), String> {
    let path = get_storage_path(&app, &key);

    if let Some(parent) = path.parent() {
        fs::create_dir_all(parent)
            .await
            .map_err(|e| format!("Failed to create storage directory: {}", e))?;
    }

    fs::write(&path, value.as_bytes())
        .await
        .map_err(|e| format!("Failed to write {}.json: {}", key, e))?;

    Ok(())
}

#[tauri::command]
pub async fn load_json(app: AppHandle, key: String) -> Result<Option<String>, String> {
    let path = get_storage_path(&app, &key);

    if !path.exists() {
        return Ok(None);
    }

    let content = fs::read_to_string(&path)
        .await
        .map_err(|e| format!("Failed to read {}.json: {}", key, e))?;

    Ok(Some(content))
}

#[tauri::command]
pub async fn delete_json(app: AppHandle, key: String) -> Result<(), String> {
    let path = get_storage_path(&app, &key);

    if path.exists() {
        fs::remove_file(&path)
            .await
            .map_err(|e| format!("Failed to delete {}.json: {}", key, e))?;
    }

    Ok(())
}

// ---------------------------------------------------------------------------
// Thumbnail caching
// ---------------------------------------------------------------------------

fn get_thumbnail_cache_dir(app: &AppHandle) -> PathBuf {
    let mut path = app
        .path()
        .app_local_data_dir()
        .unwrap_or_else(|_| PathBuf::from("."));
    path.push("thumbnail_cache");
    path
}

#[tauri::command]
pub async fn cache_thumbnail(
    app: AppHandle,
    url: String,
    job_id: String,
) -> Result<String, String> {
    let cache_dir = get_thumbnail_cache_dir(&app);
    fs::create_dir_all(&cache_dir)
        .await
        .map_err(|e| format!("Failed to create thumbnail cache dir: {}", e))?;

    // Determine extension from URL or default to jpg
    let ext = url
        .split('?')
        .next()
        .and_then(|u| u.rsplit('.').next())
        .filter(|e| ["jpg", "jpeg", "png", "webp"].contains(e))
        .unwrap_or("jpg");

    let filename = format!("{}.{}", job_id, ext);
    let dest = cache_dir.join(&filename);

    // Download the thumbnail
    let client = reqwest::Client::builder()
        .user_agent("media-archiver")
        .build()
        .map_err(|e| format!("Failed to create HTTP client: {}", e))?;

    let response = client
        .get(&url)
        .send()
        .await
        .map_err(|e| format!("Failed to download thumbnail: {}", e))?;

    let bytes = response
        .bytes()
        .await
        .map_err(|e| format!("Failed to read thumbnail data: {}", e))?;

    let mut file = fs::File::create(&dest)
        .await
        .map_err(|e| format!("Failed to create thumbnail file: {}", e))?;

    file.write_all(&bytes)
        .await
        .map_err(|e| format!("Failed to write thumbnail: {}", e))?;

    Ok(dest.to_string_lossy().to_string())
}

#[tauri::command]
pub async fn clear_thumbnail_cache(app: AppHandle) -> Result<(), String> {
    let cache_dir = get_thumbnail_cache_dir(&app);
    if cache_dir.exists() {
        fs::remove_dir_all(&cache_dir)
            .await
            .map_err(|e| format!("Failed to clear thumbnail cache: {}", e))?;
    }
    Ok(())
}
