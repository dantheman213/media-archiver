package app.models;

public class DownloadMediaViewModel {
    private DownloadMediaModel media;

    public DownloadMediaViewModel(DownloadMediaModel obj) {
        this.media = obj;
    }

    public DownloadMediaModel getMedia() {
        return media;
    }

    public String getName() {
        return media.name;
    }

    public String getTotalSizePretty() {
        return "1.0 MB";
    }

    public float getCurrentProgress() {
        return 0.1f;
    }

    public String getCurrentDownloadSpeed() {
        return "1 KB/s";
    }

    public String getEstimatedTimeToComplete() {
        return "N/A";
    }

    public String getOptions() {
        return "N/A";
    }

    public String getUrl() {
        return media.url;
    }

    public String getOutputPath() {
        return media.outputPath;
    }
}
