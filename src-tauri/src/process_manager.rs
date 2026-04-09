use crate::emit_error;
use serde::Serialize;
use std::process::Stdio;
use tauri::{AppHandle, Emitter};
use tokio::io::{AsyncBufReadExt, BufReader};
use tokio::process::{Child, Command};

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

    pub async fn spawn(&self, job_id: String, mut command: Command) -> Result<Child, String> {
        command.stdout(Stdio::piped());
        command.stderr(Stdio::piped());

        let mut child = command.spawn().map_err(|e| {
            let msg = format!("Failed to spawn process for job {}: {}", job_id, e);
            emit_error(&self.app_handle, "Process Error", &msg);
            msg
        })?;

        let stdout = child.stdout.take().ok_or("Failed to capture stdout")?;
        let stderr = child.stderr.take().ok_or("Failed to capture stderr")?;

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
        tokio::spawn(async move {
            let mut reader = BufReader::new(stderr).lines();
            while let Ok(Some(line)) = reader.next_line().await {
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

        Ok(child)
    }
}
