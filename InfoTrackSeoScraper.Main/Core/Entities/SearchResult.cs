namespace InfoTrackSeoScraper.Main.Core.Entities
{
    public class SearchResult
    {
        public int Id { get; set; }
        public string? Query { get; set; }
        public string? TargetUrl { get; set; }
        public string? SearchEngine { get; set; }
        public string? Rankings { get; set; }
        public DateTime SearchDate { get; set; }
    }
}