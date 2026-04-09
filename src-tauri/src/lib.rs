mod commands;
pub mod models;
pub mod process_manager;

use serde::Serialize;
use tauri::{AppHandle, Emitter};

#[derive(Clone, Serialize)]
pub struct AppError {
    pub title: String,
    pub message: String,
}

/// Emit a global error event that the frontend can listen for.
pub fn emit_error(app: &AppHandle, title: &str, message: &str) {
    let _ = app.emit(
        "app-error",
        AppError {
            title: title.to_string(),
            message: message.to_string(),
        },
    );
}

// Learn more about Tauri commands at https://tauri.app/develop/calling-rust/
#[tauri::command]
fn greet(name: &str) -> String {
    format!("Hello, {}! You've been greeted from Rust!", name)
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_dialog::init())
        .plugin(tauri_plugin_opener::init())
        .invoke_handler(tauri::generate_handler![
            greet,
            commands::binaries::check_binaries,
            commands::binaries::install_binaries,
            commands::binaries::set_binary_paths,
            commands::binaries::update_ytdlp,
            commands::metadata::fetch_metadata,
            commands::disk::get_default_download_dir,
            commands::disk::get_disk_space,
            commands::disk::select_directory,
            commands::disk::pick_file,
            commands::download::start_download
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
