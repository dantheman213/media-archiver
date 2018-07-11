package app.models;

public class DownloadMediaViewModel {
    private String name;
    private int totalSizeInBytes;
    private int currentDownloadSizeInBytes;
    private String options;
    private String url;
    private String outputPath;

    public DownloadMediaViewModel() {
        name = "This is a test";
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
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
        return options;
    }

    public String getUrl() {
        return url;
    }

    public void setUrl(String url) {
        this.url = url;
    }

    public String getOutputPath() {
        return outputPath;
    }
}
