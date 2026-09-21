use futures_util::StreamExt;
use serde::{Deserialize, Serialize};
use std::fs;
use std::io::{Read, Write};
use std::path::{Path, PathBuf};
use std::sync::atomic::{AtomicBool, Ordering};
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
    crate::process_env::apply_child_env(&mut cmd);

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

    // An executable can exist yet fail to run — most commonly an x86_64 build
    // on Apple Silicon with no Rosetta ("bad CPU type in executable"), or a
    // binary whose executable bit was lost. Reporting it as "Found" hides the
    // problem until yt-dlp aborts mid-download with a confusing "ffmpeg not
    // found" error. If the version probe cannot run it, treat it as missing so
    // the UI prompts a reinstall/repair instead.
    let (yt_dlp_found, yt_dlp_path) = if yt_dlp_version.is_none() {
        (false, None)
    } else {
        (yt_dlp_found, yt_dlp_path)
    };
    let (ffmpeg_found, ffmpeg_path) = if ffmpeg_version.is_none() {
        (false, None)
    } else {
        (ffmpeg_found, ffmpeg_path)
    };
    let (ffprobe_found, ffprobe_path) = if ffprobe_version.is_none() {
        (false, None)
    } else {
        (ffprobe_found, ffprobe_path)
    };

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

/// The official source Media Archiver uses to obtain each helper tool on this
/// platform. Surfaced in the Getting Started screen so "Set Up Automatically"
/// can state exactly what it will download, instead of the vague "engine" and
/// "media processor" wording.
#[derive(Serialize, Clone)]
pub struct BinarySource {
    pub name: String,
    pub url: Option<String>,
    /// Extra explanation when there is no direct URL (e.g. ffmpeg installed
    /// from the distro's package manager, or ffprobe bundled inside another
    /// download).
    pub note: Option<String>,
}

/// yt-dlp release URL for the host platform.
fn ytdlp_download_url() -> &'static str {
    #[cfg(target_os = "windows")]
    return "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";
    #[cfg(target_os = "macos")]
    return "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp_macos";
    #[cfg(target_os = "linux")]
    return "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp";
}

/// ffmpeg build URL for the host platform, or `None` on Linux where ffmpeg
/// comes from the distribution's package manager instead of a prebuilt archive.
fn ffmpeg_download_url() -> Option<&'static str> {
    #[cfg(target_os = "windows")]
    return if cfg!(target_arch = "aarch64") {
        Some("https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-winarm64-gpl.zip")
    } else {
        Some("https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip")
    };
    #[cfg(target_os = "macos")]
    return if cfg!(target_arch = "aarch64") {
        Some("https://github.com/eugeneware/ffmpeg-static/releases/latest/download/ffmpeg-darwin-arm64")
    } else {
        Some("https://github.com/eugeneware/ffmpeg-static/releases/latest/download/ffmpeg-darwin-x64")
    };
    #[cfg(target_os = "linux")]
    return None;
}

/// ffprobe is published separately only on macOS; on Windows it ships inside the
/// ffmpeg archive and on Linux it ships in the ffmpeg package.
fn ffprobe_download_url() -> Option<&'static str> {
    #[cfg(target_os = "macos")]
    return if cfg!(target_arch = "aarch64") {
        Some("https://github.com/eugeneware/ffmpeg-static/releases/latest/download/ffprobe-darwin-arm64")
    } else {
        Some("https://github.com/eugeneware/ffmpeg-static/releases/latest/download/ffprobe-darwin-x64")
    };
    #[cfg(not(target_os = "macos"))]
    return None;
}

/// AtomicParsley release URL for the host platform.
fn atomicparsley_download_url() -> &'static str {
    #[cfg(target_os = "windows")]
    return if cfg!(target_arch = "x86") {
        "https://github.com/wez/atomicparsley/releases/latest/download/AtomicParsleyWindowsX86.zip"
    } else {
        "https://github.com/wez/atomicparsley/releases/latest/download/AtomicParsleyWindows.zip"
    };
    #[cfg(target_os = "macos")]
    return "https://github.com/wez/atomicparsley/releases/latest/download/AtomicParsleyMacOS.zip";
    #[cfg(target_os = "linux")]
    return "https://github.com/wez/atomicparsley/releases/latest/download/AtomicParsleyLinux.zip";
}

/// Describe what Automatic Setup will download/install on this platform.
#[tauri::command]
pub fn get_binary_sources() -> Vec<BinarySource> {
    let ffmpeg_url = ffmpeg_download_url();
    let ffprobe_url = ffprobe_download_url();

    let ffprobe_note = if ffprobe_url.is_some() {
        None
    } else if cfg!(target_os = "windows") {
        Some("included with the ffmpeg download".to_string())
    } else {
        Some("installed together with ffmpeg".to_string())
    };

    vec![
        BinarySource {
            name: "yt-dlp".to_string(),
            url: Some(ytdlp_download_url().to_string()),
            note: None,
        },
        BinarySource {
            name: "ffmpeg".to_string(),
            url: ffmpeg_url.map(str::to_string),
            note: if ffmpeg_url.is_none() {
                Some("installed from your system package manager".to_string())
            } else {
                None
            },
        },
        BinarySource {
            name: "ffprobe".to_string(),
            url: ffprobe_url.map(str::to_string),
            note: ffprobe_note,
        },
        BinarySource {
            name: "AtomicParsley".to_string(),
            url: Some(atomicparsley_download_url().to_string()),
            note: None,
        },
    ]
}

/// Return the user's configured binary path overrides (empty options mean
/// "auto-detect").
#[tauri::command]
pub fn get_binary_paths(app: AppHandle) -> BinaryPaths {
    load_custom_paths(&app)
}

/// Sibling path used while a download is still in flight, e.g. `ffmpeg.zip.part`.
fn part_path(dest: &Path) -> PathBuf {
    let mut os = dest.as_os_str().to_os_string();
    os.push(".part");
    PathBuf::from(os)
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
    let res = client
        .get(url)
        .send()
        .await
        .map_err(|e| format!("Couldn't download {component} from {url}: {e}"))?
        // Without this a 4xx/5xx (rate limiting, a proxy error page, a moved
        // asset) is written to disk and only surfaces much later as a bogus
        // archive-format error.
        .error_for_status()
        .map_err(|e| format!("Downloading {component} from {url} failed: {e}"))?;
    let total_size = res.content_length().unwrap_or(0);

    // Stream into a `.part` file and move it into place only once the whole body
    // has arrived. Writing straight to `dest` means a connection that dies
    // mid-transfer leaves a half-written file at the path callers later treat
    // as complete — which is how a ZIP download ends up reported as
    // "invalid Zip archive: Could not find EOCD".
    let part = part_path(dest);
    let mut file = fs::File::create(&part)
        .map_err(|e| format!("Failed to create {}: {e}", part.display()))?;
    let mut downloaded: u64 = 0;
    let mut stream = res.bytes_stream();

    while let Some(item) = stream.next().await {
        let chunk = match item {
            Ok(chunk) => chunk,
            Err(e) => {
                let _ = fs::remove_file(&part);
                return Err(format!(
                    "Downloading {component} from {url} was interrupted after {downloaded} bytes: {e}"
                ));
            }
        };
        if let Err(e) = file.write_all(&chunk) {
            let _ = fs::remove_file(&part);
            return Err(format!("Failed to write {}: {e}", part.display()));
        }
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
    drop(file);

    // A short body that still ended the stream cleanly (no transport error) is
    // the other way a truncated archive reaches the extractor.
    if total_size > 0 && downloaded != total_size {
        let _ = fs::remove_file(&part);
        return Err(format!(
            "Downloading {component} from {url} was incomplete: received {downloaded} of {total_size} bytes. \
             Check your connection and try again."
        ));
    }

    // Replace the destination with the finished download. Remove first so this
    // works even where rename refuses to clobber an existing file.
    let _ = fs::remove_file(dest);
    fs::rename(&part, dest).map_err(|e| {
        let _ = fs::remove_file(&part);
        format!("Failed to move the downloaded {component} into place: {e}")
    })?;

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

/// Open a freshly downloaded archive as a ZIP, turning the zip crate's terse
/// "Could not find EOCD" into an error that names the component and URL.
///
/// A blocked, rate-limited, truncated, or proxy-served response all reach this
/// point as a file that simply isn't a ZIP; the raw crate message gives the user
/// no way to tell what went wrong or how to recover.
fn open_zip(
    path: &Path,
    url: &str,
    component: &str,
) -> Result<zip::ZipArchive<std::io::BufReader<fs::File>>, String> {
    let file = fs::File::open(path).map_err(|e| {
        format!(
            "Couldn't open the downloaded {component} archive at {}: {e}",
            path.display()
        )
    })?;
    let size = file.metadata().map(|m| m.len()).unwrap_or(0);
    let mut reader = std::io::BufReader::new(file);

    let mut magic = [0u8; 4];
    if reader.read_exact(&mut magic).is_err() {
        return Err(format!(
            "The {component} download from {url} isn't a valid ZIP archive ({size} bytes — too \
             short to read). It may have been blocked or truncated; check your connection and try again."
        ));
    }

    // Local file header, empty-archive EOCD, or spanned-archive marker.
    let is_zip = matches!(
        magic,
        [0x50, 0x4b, 0x03, 0x04] | [0x50, 0x4b, 0x05, 0x06] | [0x50, 0x4b, 0x07, 0x08]
    );
    if !is_zip {
        return Err(format!(
            "The {component} download from {url} isn't a valid ZIP archive ({size} bytes). It may \
             have been blocked, rate-limited, or replaced with an error page by your network or \
             proxy. Check your connection and try again."
        ));
    }

    zip::ZipArchive::new(reader)
        .map_err(|e| format!("The {component} archive from {url} couldn't be read: {e}"))
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
    crate::process_env::apply_child_env_async(&mut cmd);

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

/// Package managers we know how to drive, in probe order. Every common distro
/// ships ffprobe inside the `ffmpeg` package, so one install covers both.
#[cfg(target_os = "linux")]
fn linux_ffmpeg_install() -> Option<(&'static str, Vec<&'static str>)> {
    if which::which("apt-get").is_ok() {
        Some(("apt-get", vec!["install", "-y", "ffmpeg"]))
    } else if which::which("dnf").is_ok() {
        Some(("dnf", vec!["install", "-y", "ffmpeg"]))
    } else if which::which("yum").is_ok() {
        Some(("yum", vec!["install", "-y", "ffmpeg"]))
    } else if which::which("pacman").is_ok() {
        Some(("pacman", vec!["-S", "--noconfirm", "ffmpeg"]))
    } else if which::which("zypper").is_ok() {
        Some(("zypper", vec!["--non-interactive", "install", "ffmpeg"]))
    } else if which::which("apk").is_ok() {
        Some(("apk", vec!["add", "--no-cache", "ffmpeg"]))
    } else if which::which("xbps-install").is_ok() {
        Some(("xbps-install", vec!["-y", "ffmpeg"]))
    } else {
        None
    }
}

/// Install ffmpeg/ffprobe from the distro's own repositories. Native packages
/// are built against the running system's C library, so this sidesteps the
/// glibc-mismatch problem of handing out a prebuilt static binary.
#[cfg(target_os = "linux")]
async fn install_ffmpeg_linux(app: &AppHandle) -> Result<(), String> {
    let (pm, args) = linux_ffmpeg_install().ok_or_else(|| {
        "Couldn't detect a supported package manager. Install ffmpeg with your distro's \
         package manager, then choose \"I Already Have These Tools\" to point Media Archiver at it."
            .to_string()
    })?;

    let manual = format!("sudo {} {}", pm, args.join(" "));

    // pkexec sanitises PATH and may not find the manager by bare name; resolve
    // its absolute path once and use that everywhere we exec it.
    let pm_path = which::which(pm)
        .map(|p| p.to_string_lossy().to_string())
        .unwrap_or_else(|_| pm.to_string());

    // A GUI app has no TTY, so `sudo` cannot prompt for a password. pkexec
    // instead raises the desktop's graphical authentication dialog, which is
    // the right tool here. Fall back to non-interactive sudo (passwordless
    // sudo, or already root) and finally to running the manager directly.
    let mut cmd = if which::which("pkexec").is_ok() {
        let mut c = tokio::process::Command::new("pkexec");
        c.arg(&pm_path);
        c
    } else if which::which("sudo").is_ok() {
        let mut c = tokio::process::Command::new("sudo");
        c.arg("-n").arg(&pm_path);
        c
    } else {
        tokio::process::Command::new(&pm_path)
    };
    cmd.args(&args);

    let _ = app.emit(
        "download-progress",
        ProgressPayload {
            component: "ffmpeg-extract".to_string(),
            progress: 0.0,
        },
    );

    let output = cmd.output().await.map_err(|e| {
        format!("Failed to run {pm}: {e}\n\nInstall ffmpeg manually:\n\n    {manual}")
    })?;

    if !output.status.success() {
        let stderr = String::from_utf8_lossy(&output.stderr);
        let stdout = String::from_utf8_lossy(&output.stdout);
        let detail = if stderr.trim().is_empty() {
            stdout.trim()
        } else {
            stderr.trim()
        };
        let lower = detail.to_lowercase();

        // Turn the most common failures into actionable guidance rather than a
        // raw blob of package-manager output.
        let hint = if lower.contains("password is required")
            || lower.contains("no tty")
            || lower.contains("authentication agent")
            || lower.contains("not authorized")
        {
            format!(
                "Media Archiver can't prompt for a password on this system. Run this in a \
                 terminal, then try again:\n\n    {manual}"
            )
        } else if lower.contains("unable to locate package")
            || lower.contains("no match for argument")
        {
            "Your package lists may be stale or the ffmpeg package may live in a third-party \
             repository (e.g. RPM Fusion on Fedora, Packman on openSUSE). Refresh your lists or \
             enable that repository, then try again."
                .to_string()
        } else if detail.is_empty() {
            format!("No output from {pm}. Install ffmpeg manually:\n\n    {manual}")
        } else {
            format!("Install ffmpeg manually:\n\n    {manual}")
        };

        let mut msg = format!("Installing ffmpeg with {pm} failed");
        if !detail.is_empty() {
            msg.push_str(&format!(":\n{detail}"));
        }
        msg.push_str(&format!("\n\n{hint}"));
        return Err(msg);
    }

    let _ = app.emit(
        "download-progress",
        ProgressPayload {
            component: "ffmpeg-extract".to_string(),
            progress: 1.0,
        },
    );

    // Confirm the install actually produced a runnable ffmpeg/ffprobe.
    if which::which("ffmpeg").is_err() || which::which("ffprobe").is_err() {
        return Err(format!(
            "{pm} reported success but ffmpeg/ffprobe still aren't on PATH. Install them \
             manually:\n\n    {manual}"
        ));
    }

    Ok(())
}

/// Guards against two setup runs writing the same archive/binary paths at once,
/// which interleaves their bytes and corrupts the result.
static INSTALL_IN_PROGRESS: AtomicBool = AtomicBool::new(false);

struct InstallGuard;

impl Drop for InstallGuard {
    fn drop(&mut self) {
        INSTALL_IN_PROGRESS.store(false, Ordering::SeqCst);
    }
}

#[tauri::command]
pub async fn install_binaries(app: AppHandle) -> Result<(), String> {
    if INSTALL_IN_PROGRESS.swap(true, Ordering::SeqCst) {
        return Err("A setup is already in progress. Please wait for it to finish.".to_string());
    }
    let _install_guard = InstallGuard;

    let bin_dir = get_bin_dir(&app);
    fs::create_dir_all(&bin_dir).map_err(|e| e.to_string())?;

    // Download source URLs are shared with get_binary_sources() so the
    // Getting Started screen always states the same thing the installer does.
    let ytdlp_url = ytdlp_download_url();

    let yt_dlp_dest = bin_dir.join(if cfg!(target_os = "windows") {
        "yt-dlp.exe"
    } else {
        "yt-dlp"
    });

    // Download yt-dlp
    download_file(&app, ytdlp_url, &yt_dlp_dest, "yt-dlp").await?;

    // FFmpeg is harder, we'll download a simpler build.
    //
    // macOS: evermeet.cx only publishes x86_64 builds and explicitly does not
    // ship Apple Silicon binaries. Handing that Intel build to an M-series Mac
    // produces an x86_64 executable that only runs through Rosetta; when Rosetta
    // isn't installed, every invocation dies with "bad CPU type in executable".
    // yt-dlp then can't find a *working* ffmpeg and aborts postprocessing with
    // "ffmpeg not found. Please install or provide the path using
    // --ffmpeg-location" — even though the file exists. ffmpeg-static publishes
    // native, unarchived binaries for both architectures, so use it on macOS.
    //
    // Windows: BtbN publishes per-architecture zips. Match the host CPU so
    // Windows on ARM gets a native winarm64 build instead of an emulated x64
    // one. (BtbN no longer ships a 32-bit win32 build, and Tauri does not
    // target 32-bit Windows, so x86_64 is the fallback.)
    #[cfg(target_os = "windows")]
    let ffmpeg_url = ffmpeg_download_url().expect("ffmpeg download URL for Windows");
    #[cfg(target_os = "macos")]
    let ffmpeg_url = ffmpeg_download_url().expect("ffmpeg download URL for macOS");
    #[cfg(target_os = "macos")]
    let ffprobe_url = ffprobe_download_url().expect("ffprobe download URL for macOS");

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

    let _ = app.emit(
        "download-progress",
        ProgressPayload {
            component: "ffmpeg-extract".to_string(),
            progress: 0.0,
        },
    );

    #[cfg(target_os = "macos")]
    {
        // Raw (non-archived) binaries: download straight to their final paths.
        // download_file() marks each unix download executable.
        download_file(&app, ffmpeg_url, &ffmpeg_dest, "ffmpeg").await?;
        download_file(&app, ffprobe_url, &ffprobe_dest, "ffmpeg").await?;
        if !ffmpeg_dest.exists() || !ffprobe_dest.exists() {
            return Err(format!(
                "The ffmpeg/ffprobe download ({ffmpeg_url}) didn't produce usable binaries. \
                 Install them manually and use \"I Already Have These Tools\"."
            ));
        }
    }

    #[cfg(target_os = "windows")]
    {
        let ffmpeg_archive = bin_dir.join("ffmpeg.zip");
        download_file(&app, ffmpeg_url, &ffmpeg_archive, "ffmpeg").await?;
        // Extract Windows zip — pull BOTH ffmpeg.exe and ffprobe.exe (the BtbN
        // build ships both under bin/). Don't stop after the first match.
        let mut archive = open_zip(&ffmpeg_archive, ffmpeg_url, "ffmpeg")?;
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
        let _ = fs::remove_file(ffmpeg_archive);
        if !ffmpeg_dest.exists() || !ffprobe_dest.exists() {
            return Err(format!(
                "The ffmpeg archive from {ffmpeg_url} downloaded, but didn't contain both \
                 ffmpeg.exe and ffprobe.exe. Install ffmpeg manually and use \"I Already Have \
                 These Tools\"."
            ));
        }
    }

    #[cfg(target_os = "linux")]
    {
        // No prebuilt binary is downloaded on Linux. Instead install ffmpeg
        // (which provides ffprobe) from the distro's own repositories, built
        // against the running system's C library.
        install_ffmpeg_linux(&app).await?;
    }

    #[cfg(unix)]
    {
        use std::os::unix::fs::PermissionsExt;
        // Mark both binaries executable. ffprobe in particular loses its
        // executable bit when it is extracted from a zip via fs::File::create,
        // and yt-dlp refuses to use a non-executable helper.
        for dest in [&ffmpeg_dest, &ffprobe_dest] {
            if let Ok(metadata) = fs::metadata(dest) {
                let mut perms = metadata.permissions();
                perms.set_mode(0o755);
                let _ = fs::set_permissions(dest, perms);
            }
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
    //
    // wez/atomicparsley publishes a 64-bit Windows build and a 32-bit
    // (WindowsX86) build, but no ARM64 build. On Windows-on-ARM the 64-bit
    // build runs under x64 emulation, which is acceptable for this optional
    // helper. Pick the 32-bit build only when the host itself is 32-bit.
    let atomicparsley_url = atomicparsley_download_url();

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
        let mut archive = open_zip(&atomicparsley_archive, atomicparsley_url, "AtomicParsley")?;
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

    if !atomicparsley_dest.exists() {
        return Err(format!(
            "The AtomicParsley archive from {atomicparsley_url} downloaded, but didn't contain \
             AtomicParsley. Install it manually and use \"I Already Have These Tools\"."
        ));
    }

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
