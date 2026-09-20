use crate::emit_error;
use chrono::Local;
use serde::Serialize;
use std::process::Stdio;
use std::sync::{Arc, Mutex};
use tauri::{AppHandle, Emitter};
use tokio::fs::OpenOptions;
use tokio::io::{AsyncBufReadExt, AsyncWriteExt, BufReader};
use tokio::process::{Child, Command};
use tokio::sync::Mutex as AsyncMutex;

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

/// Writes the raw stdout/stderr of a single ingestion to a per-job log file so
/// failures can be diagnosed even when the UI only shows a generic error.
#[derive(Clone)]
pub struct JobLogger {
    file: Arc<AsyncMutex<tokio::fs::File>>,
}

impl JobLogger {
    async fn create(
        app: &AppHandle,
        job_id: &str,
        label: Option<&str>,
        command: &Command,
    ) -> Option<Self> {
        let dir = crate::commands::logs::logs_dir(app);
        tokio::fs::create_dir_all(&dir).await.ok()?;

        let path = dir.join(format!("{}.log", job_id));
        let mut file = OpenOptions::new()
            .create(true)
            .append(true)
            .open(&path)
            .await
            .ok()?;

        let program = command.as_std().get_program().to_string_lossy().to_string();
        let args = command
            .as_std()
            .get_args()
            .map(|a| a.to_string_lossy().to_string())
            .collect::<Vec<_>>()
            .join(" ");

        let mut header = String::new();
        header.push_str("==================================================\n");
        header.push_str("Media Archiver ingestion log\n");
        header.push_str(&format!("job_id : {}\n", job_id));
        header.push_str(&format!(
            "started: {}\n",
            Local::now().format("%Y-%m-%d %H:%M:%S")
        ));
        if let Some(label) = label {
            header.push_str(&format!("source : {}\n", label));
        }
        header.push_str(&format!("command: {} {}\n", program, args));
        header.push_str("==================================================\n");

        file.write_all(header.as_bytes()).await.ok()?;

        Some(Self {
            file: Arc::new(AsyncMutex::new(file)),
        })
    }

    async fn write(&self, stream: &str, line: &str) {
        let mut file = self.file.lock().await;
        let _ = file
            .write_all(format!("[{}] {}\n", stream, line).as_bytes())
            .await;
    }

    /// Write a plain line without a stream prefix (used for headers/footers).
    pub async fn note(&self, text: &str) {
        let mut file = self.file.lock().await;
        let _ = file.write_all(format!("{}\n", text).as_bytes()).await;
    }
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
        log_label: Option<String>,
        collect_logs: bool,
    ) -> Result<(Child, StderrTail, Option<JobLogger>), String> {
        command.stdout(Stdio::piped());
        command.stderr(Stdio::piped());

        let logger = if collect_logs {
            JobLogger::create(&self.app_handle, &job_id, log_label.as_deref(), &command).await
        } else {
            None
        };

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
        let stdout_logger = logger.clone();
        tokio::spawn(async move {
            let mut reader = BufReader::new(stdout).lines();
            while let Ok(Some(line)) = reader.next_line().await {
                if let Some(ref logger) = stdout_logger {
                    logger.write("stdout", &line).await;
                }
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
        let stderr_logger = logger.clone();
        tokio::spawn(async move {
            let mut reader = BufReader::new(stderr).lines();
            while let Ok(Some(line)) = reader.next_line().await {
                if let Some(ref logger) = stderr_logger {
                    logger.write("stderr", &line).await;
                }
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

        Ok((child, stderr_tail, logger))
    }
}
