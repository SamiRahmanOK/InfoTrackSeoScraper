namespace InfoTrackSeoScraper.Main.Infrastructure.Settings
{
    public class SearchEngineSettings
    {
        public int MaxRetries { get; set; } = 3;
        public int RequestTimeoutSeconds { get; set; } = 30;
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
    }
}