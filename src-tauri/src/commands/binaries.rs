use futures_util::StreamExt;
use serde::{Deserialize, Serialize};
use std::fs;
use std::io::Write;
use std::path::{Path, PathBuf};
use tauri::{AppHandle, Emitter, Manager};

#[derive(Serialize, Deserialize, Clone)]
pub struct BinaryStatus {
    pub yt_dlp_found: bool,
    pub yt_dlp_path: Option<String>,
    pub yt_dlp_version: Option<String>,
    pub ffmpeg_found: bool,
    pub ffmpeg_path: Option<String>,
    pub ffmpeg_version: Option<String>,
    pub ffprobe_found: bool,
    pub ffprobe_path: Option<String>,
    pub ffprobe_version: Option<String>,
    pub atomic_parsley_found: bool,
    pub atomic_parsley_path: Option<String>,
    pub atomic_parsley_version: Option<String>,
}

#[derive(Serialize, Deserialize)]
pub struct BinaryPaths {
    pub yt_dlp_path: Option<String>,
    pub ffmpeg_path: Option<String>,
    #[serde(default)]
    pub ffprobe_path: Option<String>,
    #[serde(default)]
    pub atomic_parsley_path: Option<String>,
}

#[derive(Clone, Serialize)]
struct ProgressPayload {
    component: String,
    progress: f32, // 0.0 to 1.0
}

fn get_bin_dir(app: &AppHandle) -> PathBuf {
    let mut path = app
        .path()
        .app_local_data_dir()
        .unwrap_or_else(|_| PathBuf::from("."));
    path.push("bin");
    path
}

fn get_config_path(app: &AppHandle) -> PathBuf {
    let mut path = app
        .path()
        .app_local_data_dir()
        .unwrap_or_else(|_| PathBuf::from("."));
    path.push("binary_paths.json");
    path
}

fn load_custom_paths(app: &AppHandle) -> BinaryPaths {
    let config_path = get_config_path(app);
    if config_path.exists() {
        if let Ok(content) = fs::read_to_string(config_path) {
            if let Ok(paths) = serde_json::from_str::<BinaryPaths>(&content) {
                return paths;
            }
        }
    }
    BinaryPaths {
        yt_dlp_path: None,
        ffmpeg_path: None,
        ffprobe_path: None,
        atomic_parsley_path: None,
    }
}

/// ffprobe almost always sits next to ffmpeg. When ffprobe isn't found through
/// the normal resolution order, look in the same directory as the resolved
/// ffmpeg binary (covers a custom ffmpeg path whose sibling holds ffprobe).
fn ffprobe_beside_ffmpeg(ffmpeg_path: &Option<String>) -> Option<String> {
    let ffmpeg = ffmpeg_path.as_ref()?;
    let dir = Path::new(ffmpeg).parent()?;
    let exe = if cfg!(target_os = "windows") {
        "ffprobe.exe"
    } else {
        "ffprobe"
    };
    let candidate = dir.join(exe);
    if candidate.exists() {
        Some(candidate.to_string_lossy().to_string())
    } else {
        None
    }
}

fn get_version(exe_path: &str, args: &[&str]) -> Option<String> {
    let mut cmd = std::process::Command::new(exe_path);
    cmd.args(args);

    #[cfg(target_os = "windows")]
    {
        use std::os::windows::process::CommandExt;
        cmd.creation_flags(0x08000000);
    }

    cmd.output()
        .ok()
        .and_then(|output| {
            if output.status.success() {
                let stdout = String::from_utf8_lossy(&output.stdout).to_string();
                // yt-dlp --version returns just a version string like "2024.01.01"
                // ffmpeg -version returns "ffmpeg version N-xxxxx ..." on the first line
                let first_line = stdout.lines().next().unwrap_or("").trim().to_string();
                if first_line.is_empty() {
                    None
                } else {
                    Some(first_line)
                }
            } else {
                None
            }
        })
}

fn check_executable(
    name: &str,
    custom_path: &Option<String>,
    bin_dir: &Path,
) -> (bool, Option<String>) {
    // 1. Check custom path
    if let Some(path_str) = custom_path {
        let p = Path::new(path_str);
        if p.exists() {
            return (true, Some(path_str.clone()));
        }
    }

    // 2. Check local bin dir
    let exe_name = if cfg!(target_os = "windows") {
        format!("{}.exe", name)
    } else {
        name.to_string()
    };
    let local_path = bin_dir.join(&exe_name);
    if local_path.exists() {
        return (true, Some(local_path.to_string_lossy().to_string()));
    }

    // 3. Check system PATH
    if let Ok(path) = which::which(name) {
        return (true, Some(path.to_string_lossy().to_string()));
    }
    if let Ok(path) = which::which(&exe_name) {
        return (true, Some(path.to_string_lossy().to_string()));
    }

    (false, None)
}

#[tauri::command]
pub fn check_binaries(app: AppHandle) -> BinaryStatus {
    let bin_dir = get_bin_dir(&app);
    let paths = load_custom_paths(&app);

    let (yt_dlp_found, yt_dlp_path) = check_executable("yt-dlp", &paths.yt_dlp_path, &bin_dir);
    let (ffmpeg_found, ffmpeg_path) = check_executable("ffmpeg", &paths.ffmpeg_path, &bin_dir);
    let (mut ffprobe_found, mut ffprobe_path) =
        check_executable("ffprobe", &paths.ffprobe_path, &bin_dir);
    // Fallback: ffprobe living beside the resolved ffmpeg binary.
    if !ffprobe_found {
        if let Some(p) = ffprobe_beside_ffmpeg(&ffmpeg_path) {
            ffprobe_found = true;
            ffprobe_path = Some(p);
        }
    }
    let (atomic_parsley_found, atomic_parsley_path) =
        check_executable("AtomicParsley", &paths.atomic_parsley_path, &bin_dir);

    let yt_dlp_version = yt_dlp_path
        .as_ref()
        .and_then(|p| get_version(p, &["--version"]));

    let ffmpeg_version = ffmpeg_path
        .as_ref()
        .and_then(|p| get_version(p, &["-version"]));

    let ffprobe_version = ffprobe_path
        .as_ref()
        .and_then(|p| get_version(p, &["-version"]));

    let atomic_parsley_version = atomic_parsley_path
        .as_ref()
        .and_then(|p| get_version(p, &["--version"]));

    BinaryStatus {
        yt_dlp_found,
        yt_dlp_path,
        yt_dlp_version,
        ffmpeg_found,
        ffmpeg_path,
        ffmpeg_version,
        ffprobe_found,
        ffprobe_path,
        ffprobe_version,
        atomic_parsley_found,
        atomic_parsley_path,
        atomic_parsley_version,
    }
}

#[tauri::command]
pub fn set_binary_paths(
    app: AppHandle,
    yt_dlp_path: Option<String>,
    ffmpeg_path: Option<String>,
    ffprobe_path: Option<String>,
    atomic_parsley_path: Option<String>,
) -> Result<(), String> {
    let paths = BinaryPaths {
        yt_dlp_path,
        ffmpeg_path,
        ffprobe_path,
        atomic_parsley_path,
    };
    let config_path = get_config_path(&app);

    if let Some(parent) = config_path.parent() {
        fs::create_dir_all(parent).map_err(|e| e.to_string())?;
    }

    let content = serde_json::to_string(&paths).map_err(|e| e.to_string())?;
    fs::write(config_path, content).map_err(|e| e.to_string())?;
    Ok(())
}

async fn download_file(
    app: &AppHandle,
    url: &str,
    dest: &Path,
    component: &str,
) -> Result<(), String> {
    let client = reqwest::Client::builder()
        .user_agent("media-archiver")
        .build()
        .map_err(|e| e.to_string())?;
    let res = client.get(url).send().await.map_err(|e| e.to_string())?;
    let total_size = res.content_length().unwrap_or(0);

    let mut file = fs::File::create(dest).map_err(|e| e.to_string())?;
    let mut downloaded: u64 = 0;
    let mut stream = res.bytes_stream();

    while let Some(item) = stream.next().await {
        let chunk = item.map_err(|e| e.to_string())?;
        file.write_all(&chunk).map_err(|e| e.to_string())?;
        downloaded += chunk.len() as u64;

        if total_size > 0 {
            let progress = (downloaded as f32) / (total_size as f32);
            let _ = app.emit(
                "download-progress",
                ProgressPayload {
                    component: component.to_string(),
                    progress,
                },
            );
        }
    }

    // Make executable on unix
    #[cfg(unix)]
    {
        use std::os::unix::fs::PermissionsExt;
        let mut perms = fs::metadata(dest).map_err(|e| e.to_string())?.permissions();
        perms.set_mode(0o755);
        fs::set_permissions(dest, perms).map_err(|e| e.to_string())?;
    }

    Ok(())
}

#[tauri::command]
pub async fn update_ytdlp(app: AppHandle) -> Result<String, String> {
    let status = check_binaries(app.clone());
    let yt_dlp_path = status
        .yt_dlp_path
        .ok_or_else(|| "yt-dlp not found. Please install it first.".to_string())?;
    let version_before = status.yt_dlp_version.clone().unwrap_or_default();

    let mut cmd = tokio::process::Command::new(&yt_dlp_path);
    cmd.arg("-U");

    #[cfg(target_os = "windows")]
    {
        cmd.creation_flags(0x08000000);
    }

    let output = cmd
        .output()
        .await
        .map_err(|e| format!("Failed to run yt-dlp update: {}", e))?;

    let stdout = String::from_utf8_lossy(&output.stdout).to_string();
    let stderr = String::from_utf8_lossy(&output.stderr).to_string();
    let combined = format!("{}\n{}", stdout.trim(), stderr.trim());
    let combined = combined.trim().to_string();
    let combined_lower = combined.to_lowercase();

    // Re-read the version after the attempt so we can report the truth rather
    // than trusting a fragile stdout string.
    let version_after = get_version(&yt_dlp_path, &["--version"]).unwrap_or_default();

    if !output.status.success() {
        // Most common non-zero cause: yt-dlp was installed by a package manager
        // and refuses to self-update.
        if combined_lower.contains("package manager")
            || combined_lower.contains("pip")
            || combined_lower.contains("installed by")
        {
            return Err(format!(
                "yt-dlp at {} can't self-update — it appears to be managed by a package manager. Update it with that tool.\n{}",
                yt_dlp_path, combined
            ));
        }
        return Err(format!(
            "yt-dlp update failed at {}:\n{}",
            yt_dlp_path, combined
        ));
    }

    // Success exit — report based on the actual version change, not guesswork.
    if !version_before.is_empty() && !version_after.is_empty() && version_before != version_after {
        Ok(format!(
            "Updated yt-dlp {} \u{2192} {}\n({})",
            version_before, version_after, yt_dlp_path
        ))
    } else if combined_lower.contains("up to date") || combined_lower.contains("up-to-date") {
        Ok(format!(
            "yt-dlp is already up to date ({}).\n{}",
            version_after, yt_dlp_path
        ))
    } else {
        // Exited zero but the version didn't change: don't claim success —
        // show exactly what yt-dlp reported.
        Ok(format!(
            "yt-dlp reported no version change (still {}).\n{}\n{}",
            version_after, yt_dlp_path, combined
        ))
    }
}

#[tauri::command]
pub async fn install_binaries(app: AppHandle) -> Result<(), String> {
    let bin_dir = get_bin_dir(&app);
    fs::create_dir_all(&bin_dir).map_err(|e| e.to_string())?;

    // Define URLs based on OS
    #[cfg(target_os = "windows")]
    let ytdlp_url = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";
    #[cfg(target_os = "macos")]
    let ytdlp_url = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp_macos";
    #[cfg(target_os = "linux")]
    let ytdlp_url = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp";

    let yt_dlp_dest = bin_dir.join(if cfg!(target_os = "windows") {
        "yt-dlp.exe"
    } else {
        "yt-dlp"
    });

    // Download yt-dlp
    download_file(&app, ytdlp_url, &yt_dlp_dest, "yt-dlp").await?;

    // FFmpeg is harder, we'll download a simpler build
    #[cfg(target_os = "windows")]
    let ffmpeg_url = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip";
    #[cfg(target_os = "macos")]
    let ffmpeg_url = "https://evermeet.cx/ffmpeg/getrelease/zip";
    #[cfg(target_os = "linux")]
    let ffmpeg_url = "https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz";

    let ffmpeg_dest = bin_dir.join(if cfg!(target_os = "windows") {
        "ffmpeg.exe"
    } else {
        "ffmpeg"
    });
    // ffprobe sits beside ffmpeg; yt-dlp needs it for several post-processors.
    let ffprobe_dest = bin_dir.join(if cfg!(target_os = "windows") {
        "ffprobe.exe"
    } else {
        "ffprobe"
    });
    let ffmpeg_archive = bin_dir.join(if cfg!(target_os = "linux") {
        "ffmpeg.tar.xz"
    } else {
        "ffmpeg.zip"
    });

    download_file(&app, ffmpeg_url, &ffmpeg_archive, "ffmpeg").await?;

    let _ = app.emit(
        "download-progress",
        ProgressPayload {
            component: "ffmpeg-extract".to_string(),
            progress: 0.0,
        },
    );

    #[cfg(target_os = "windows")]
    {
        // Extract Windows zip — pull BOTH ffmpeg.exe and ffprobe.exe (the BtbN
        // build ships both under bin/). Don't stop after the first match.
        let file = fs::File::open(&ffmpeg_archive).map_err(|e| e.to_string())?;
        let mut archive = zip::ZipArchive::new(file).map_err(|e| e.to_string())?;
        for i in 0..archive.len() {
            let mut file = archive.by_index(i).map_err(|e| e.to_string())?;
            if let Some(path) = file.enclosed_name() {
                match path.file_name().and_then(|n| n.to_str()) {
                    Some("ffmpeg.exe") => {
                        let mut out = fs::File::create(&ffmpeg_dest).map_err(|e| e.to_string())?;
                        std::io::copy(&mut file, &mut out).map_err(|e| e.to_string())?;
                    }
                    Some("ffprobe.exe") => {
                        let mut out = fs::File::create(&ffprobe_dest).map_err(|e| e.to_string())?;
                        std::io::copy(&mut file, &mut out).map_err(|e| e.to_string())?;
                    }
                    _ => {}
                }
            }
        }
    }

    #[cfg(not(target_os = "windows"))]
    {
        // For macOS: each evermeet zip contains a single binary. ffmpeg and
        // ffprobe are separate downloads, so fetch both.
        #[cfg(target_os = "macos")]
        {
            // ffmpeg (already downloaded into ffmpeg_archive above)
            let file = fs::File::open(&ffmpeg_archive).map_err(|e| e.to_string())?;
            let mut archive = zip::ZipArchive::new(file).map_err(|e| e.to_string())?;
            for i in 0..archive.len() {
                let mut file = archive.by_index(i).map_err(|e| e.to_string())?;
                if let Some(path) = file.enclosed_name() {
                    if path.file_name().and_then(|n| n.to_str()) == Some("ffmpeg") {
                        let mut out = fs::File::create(&ffmpeg_dest).map_err(|e| e.to_string())?;
                        std::io::copy(&mut file, &mut out).map_err(|e| e.to_string())?;
                        break;
                    }
                }
            }

            // ffprobe (separate download)
            let ffprobe_url = "https://evermeet.cx/ffmpeg/getrelease/ffprobe/zip";
            let ffprobe_archive = bin_dir.join("ffprobe.zip");
            download_file(&app, ffprobe_url, &ffprobe_archive, "ffmpeg").await?;
            let file = fs::File::open(&ffprobe_archive).map_err(|e| e.to_string())?;
            let mut archive = zip::ZipArchive::new(file).map_err(|e| e.to_string())?;
            for i in 0..archive.len() {
                let mut file = archive.by_index(i).map_err(|e| e.to_string())?;
                if let Some(path) = file.enclosed_name() {
                    if path.file_name().and_then(|n| n.to_str()) == Some("ffprobe") {
                        let mut out = fs::File::create(&ffprobe_dest).map_err(|e| e.to_string())?;
                        std::io::copy(&mut file, &mut out).map_err(|e| e.to_string())?;
                        break;
                    }
                }
            }
            let _ = fs::remove_file(ffprobe_archive);
        }
        #[cfg(target_os = "linux")]
        {
            // The Linux static build is distributed as .tar.xz, which we can't
            // decompress without an xz decoder. Rather than leave a broken dummy
            // binary (which fails cryptically later when yt-dlp calls ffmpeg),
            // fail clearly so the user installs ffmpeg/ffprobe themselves.
            let _ = fs::remove_file(&ffmpeg_archive);
            return Err(
                "Automatic ffmpeg install isn't supported on Linux yet. Please install ffmpeg and ffprobe (e.g. via your package manager) and set their paths on the Dependencies screen."
                    .to_string(),
            );
        }
    }

    // Cleanup archive
    let _ = fs::remove_file(ffmpeg_archive);

    #[cfg(unix)]
    {
        use std::os::unix::fs::PermissionsExt;
        if let Ok(metadata) = fs::metadata(&ffmpeg_dest) {
            let mut perms = metadata.permissions();
            perms.set_mode(0o755);
            let _ = fs::set_permissions(&ffmpeg_dest, perms);
        }
    }

    let _ = app.emit(
        "download-progress",
        ProgressPayload {
            component: "ffmpeg-extract".to_string(),
            progress: 1.0,
        },
    );

    // Download AtomicParsley
    #[cfg(target_os = "windows")]
    let atomicparsley_url =
        "https://github.com/wez/atomicparsley/releases/latest/download/AtomicParsleyWindows.zip";
    #[cfg(target_os = "macos")]
    let atomicparsley_url =
        "https://github.com/wez/atomicparsley/releases/latest/download/AtomicParsleyMacOS.zip";
    #[cfg(target_os = "linux")]
    let atomicparsley_url =
        "https://github.com/wez/atomicparsley/releases/latest/download/AtomicParsleyLinux.zip";

    let atomicparsley_dest = bin_dir.join(if cfg!(target_os = "windows") {
        "AtomicParsley.exe"
    } else {
        "AtomicParsley"
    });
    let atomicparsley_archive = bin_dir.join("atomicparsley.zip");

    download_file(&app, atomicparsley_url, &atomicparsley_archive, "atomicparsley").await?;

    {
        let target_name = if cfg!(target_os = "windows") {
            "AtomicParsley.exe"
        } else {
            "AtomicParsley"
        };
        let file = fs::File::open(&atomicparsley_archive).map_err(|e| e.to_string())?;
        let mut archive = zip::ZipArchive::new(file).map_err(|e| e.to_string())?;
        for i in 0..archive.len() {
            let mut entry = archive.by_index(i).map_err(|e| e.to_string())?;
            if let Some(path) = entry.enclosed_name() {
                if path.file_name().and_then(|n| n.to_str()) == Some(target_name) {
                    let mut out =
                        fs::File::create(&atomicparsley_dest).map_err(|e| e.to_string())?;
                    std::io::copy(&mut entry, &mut out).map_err(|e| e.to_string())?;
                    break;
                }
            }
        }
    }

    let _ = fs::remove_file(atomicparsley_archive);

    #[cfg(unix)]
    {
        use std::os::unix::fs::PermissionsExt;
        if let Ok(metadata) = fs::metadata(&atomicparsley_dest) {
            let mut perms = metadata.permissions();
            perms.set_mode(0o755);
            let _ = fs::set_permissions(&atomicparsley_dest, perms);
        }
    }

    Ok(())
}

#[derive(Serialize, Clone)]
pub struct YtDlpUpdateInfo {
    pub current_version: String,
    pub latest_version: String,
    pub update_available: bool,
}

#[tauri::command]
pub async fn check_ytdlp_update(app: AppHandle) -> Result<Option<YtDlpUpdateInfo>, String> {
    let status = check_binaries(app.clone());
    let current_version = match status.yt_dlp_version {
        Some(v) => v,
        None => return Ok(None),
    };

    let client = reqwest::Client::builder()
        .user_agent("media-archiver")
        .build()
        .map_err(|e| e.to_string())?;

    // Surface failures (network, rate limiting) instead of silently returning
    // "no update" — a swallowed error is why the check appeared not to work.
    let resp = client
        .get("https://api.github.com/repos/yt-dlp/yt-dlp/releases/latest")
        .send()
        .await
        .map_err(|e| format!("Failed to reach GitHub: {}", e))?
        .error_for_status()
        .map_err(|e| format!("GitHub update check failed: {}", e))?;

    let json: serde_json::Value = resp
        .json()
        .await
        .map_err(|e| format!("Failed to parse GitHub response: {}", e))?;

    let latest_version = json["tag_name"].as_str().unwrap_or("").trim().to_string();

    if latest_version.is_empty() {
        return Err(
            "Could not determine the latest yt-dlp version (GitHub API may be rate-limited)."
                .to_string(),
        );
    }

    // Normalize before comparing so cosmetic differences don't cause false
    // "update available" flags.
    let norm = |s: &str| s.trim().trim_start_matches('v').to_lowercase();
    let update_available = norm(&current_version) != norm(&latest_version);

    Ok(Some(YtDlpUpdateInfo {
        update_available,
        current_version,
        latest_version,
    }))
}
