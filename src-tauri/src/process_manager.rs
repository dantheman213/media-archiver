use crate::emit_error;
use serde::Serialize;
use std::process::Stdio;
use std::sync::{Arc, Mutex};
use tauri::{AppHandle, Emitter};
use tokio::io::{AsyncBufReadExt, BufReader};
use tokio::process::{Child, Command};

/// How many trailing stderr lines to retain for error reporting.
const STDERR_TAIL_LINES: usize = 30;

/// A shared, bounded buffer holding the most recent stderr lines of a process.
pub type StderrTail = Arc<Mutex<Vec<String>>>;

#[derive(Clone, Serialize)]
pub struct ProcessEvent {
    pub job_id: String,
    pub event_type: String, // "stdout", "stderr", "exit", "error"
    pub payload: String,
}

pub struct ProcessManager {
    app_handle: AppHandle,
}

impl ProcessManager {
    pub fn new(app_handle: AppHandle) -> Self {
        Self { app_handle }
    }

    pub async fn spawn(
        &self,
        job_id: String,
        mut command: Command,
    ) -> Result<(Child, StderrTail), String> {
        command.stdout(Stdio::piped());
        command.stderr(Stdio::piped());

        let mut child = command.spawn().map_err(|e| {
            let msg = format!("Failed to spawn process for job {}: {}", job_id, e);
            emit_error(&self.app_handle, "Process Error", &msg);
            msg
        })?;

        let stdout = child.stdout.take().ok_or("Failed to capture stdout")?;
        let stderr = child.stderr.take().ok_or("Failed to capture stderr")?;

        // Retain the most recent stderr lines so a failed job can report the
        // real yt-dlp/ffmpeg error, not just an exit code.
        let stderr_tail: StderrTail = Arc::new(Mutex::new(Vec::new()));

        let app_handle_clone = self.app_handle.clone();
        let job_id_clone = job_id.clone();
        tokio::spawn(async move {
            let mut reader = BufReader::new(stdout).lines();
            while let Ok(Some(line)) = reader.next_line().await {
                let _ = app_handle_clone.emit(
                    &format!("process-event-{}", job_id_clone),
                    ProcessEvent {
                        job_id: job_id_clone.clone(),
                        event_type: "stdout".to_string(),
                        payload: line,
                    },
                );
            }
        });

        let app_handle_clone2 = self.app_handle.clone();
        let job_id_clone2 = job_id.clone();
        let stderr_tail_clone = stderr_tail.clone();
        tokio::spawn(async move {
            let mut reader = BufReader::new(stderr).lines();
            while let Ok(Some(line)) = reader.next_line().await {
                // Keep a bounded tail of stderr for error reporting.
                if let Ok(mut buf) = stderr_tail_clone.lock() {
                    buf.push(line.clone());
                    let len = buf.len();
                    if len > STDERR_TAIL_LINES {
                        buf.drain(0..len - STDERR_TAIL_LINES);
                    }
                }
                let _ = app_handle_clone2.emit(
                    &format!("process-event-{}", job_id_clone2),
                    ProcessEvent {
                        job_id: job_id_clone2.clone(),
                        event_type: "stderr".to_string(),
                        payload: line,
                    },
                );
            }
        });

        Ok((child, stderr_tail))
    }
}
