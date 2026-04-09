use serde::{Deserialize, Serialize};

fn default_string() -> String {
    "".to_string()
}

#[derive(Debug, Serialize, Deserialize, Clone)]
#[serde(rename_all = "camelCase")]
pub struct MediaMetadata {
    pub title: String,
    #[serde(alias = "thumbnail", default = "default_string")]
    pub thumbnail_url: String,
    #[serde(alias = "duration", default)]
    pub duration_seconds: f64,
    #[serde(default = "default_string")]
    pub uploader: String,
    #[serde(default = "default_string")]
    pub description: String,
    #[serde(default = "default_string")]
    pub extractor: String,
    #[serde(alias = "formats", default)]
    pub available_formats: Vec<MediaFormat>,
}

#[derive(Debug, Serialize, Deserialize, Clone)]
#[serde(rename_all = "camelCase")]
pub struct MediaFormat {
    #[serde(alias = "format_id")]
    pub format_id: String,
    pub ext: String,
    #[serde(default = "default_string")]
    pub resolution: String,
    #[serde(default)]
    pub fps: Option<f64>,
    #[serde(default = "default_string")]
    pub vcodec: String,
    #[serde(default = "default_string")]
    pub acodec: String,
    #[serde(default)]
    pub filesize: Option<u64>,
    #[serde(alias = "filesize_approx", default)]
    pub filesize_approx: Option<u64>,
}
