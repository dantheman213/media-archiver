use serde::{Deserialize, Deserializer, Serialize};

fn null_to_string<'de, D>(d: D) -> Result<String, D::Error>
where
    D: Deserializer<'de>,
{
    Ok(Option::<String>::deserialize(d)?.unwrap_or_default())
}

#[derive(Debug, Serialize, Deserialize, Clone)]
#[serde(rename_all = "camelCase")]
pub struct MediaMetadata {
    #[serde(default, deserialize_with = "null_to_string")]
    pub title: String,
    #[serde(alias = "thumbnail", default, deserialize_with = "null_to_string")]
    pub thumbnail_url: String,
    #[serde(alias = "duration", default)]
    pub duration_seconds: f64,
    #[serde(default, deserialize_with = "null_to_string")]
    pub uploader: String,
    #[serde(default, deserialize_with = "null_to_string")]
    pub description: String,
    #[serde(default, deserialize_with = "null_to_string")]
    pub extractor: String,
    #[serde(alias = "formats", default)]
    pub available_formats: Vec<MediaFormat>,
}

#[derive(Debug, Serialize, Deserialize, Clone)]
#[serde(rename_all = "camelCase")]
pub struct MediaFormat {
    #[serde(alias = "format_id", default, deserialize_with = "null_to_string")]
    pub format_id: String,
    #[serde(default, deserialize_with = "null_to_string")]
    pub ext: String,
    #[serde(default, deserialize_with = "null_to_string")]
    pub resolution: String,
    #[serde(default)]
    pub fps: Option<f64>,
    #[serde(default, deserialize_with = "null_to_string")]
    pub vcodec: String,
    #[serde(default, deserialize_with = "null_to_string")]
    pub acodec: String,
    #[serde(default)]
    pub filesize: Option<u64>,
    #[serde(alias = "filesize_approx", default)]
    pub filesize_approx: Option<u64>,
}
