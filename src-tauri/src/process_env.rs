//! Environment hardening for child processes.
//!
//! yt-dlp is a Python program that walks every `PATH` entry looking for helper
//! executables. On Windows an entry that is a reparse point (junction/symlink)
//! can be flagged by the OS as an "untrusted mount point"; traversing it fails
//! with `ERROR_UNTRUSTED_MOUNT_POINT` (448), which Python surfaces as a fatal
//! `OSError` and aborts the whole run — even though the entry has nothing to do
//! with media archiving.
//!
//! Media Archiver has no reason to hand the user's full `PATH` to yt-dlp, so
//! children get a minimal, controlled one: just the standard Windows system
//! directories. Our own ffmpeg/ffprobe are passed explicitly with
//! `--ffmpeg-location`, so yt-dlp never needs to search `PATH` for them.

/// Build the minimal `PATH` handed to child processes on Windows. Anything the
/// user happens to have on their `PATH` (dev tools, package managers, junctions
/// into other volumes) is intentionally omitted.
#[cfg(target_os = "windows")]
fn child_path() -> Option<std::ffi::OsString> {
    use std::path::PathBuf;

    let root = std::env::var_os("SystemRoot")
        .or_else(|| std::env::var_os("windir"))
        .map(PathBuf::from)
        .unwrap_or_else(|| PathBuf::from(r"C:\Windows"));

    let system32 = root.join("System32");
    let dirs = [
        system32.clone(),
        root,
        system32.join("Wbem"),
        system32.join("WindowsPowerShell").join("v1.0"),
    ];

    std::env::join_paths(dirs).ok()
}

/// Harden a child process's environment before spawning it. No-op off Windows.
pub fn apply_child_env(cmd: &mut std::process::Command) {
    #[cfg(target_os = "windows")]
    if let Some(path) = child_path() {
        // Windows environment names are case-insensitive; clear any inherited
        // variant so the child cannot end up with two PATH entries.
        cmd.env_remove("Path");
        cmd.env_remove("PATH");
        cmd.env("PATH", path);
    }
}

/// Async-command counterpart of [`apply_child_env`].
pub fn apply_child_env_async(cmd: &mut tokio::process::Command) {
    #[cfg(target_os = "windows")]
    if let Some(path) = child_path() {
        cmd.env_remove("Path");
        cmd.env_remove("PATH");
        cmd.env("PATH", path);
    }
}
