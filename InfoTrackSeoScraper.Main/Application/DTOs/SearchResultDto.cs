namespace InfoTrackSeoScraper.Main.Application.DTOs
{
    public class SearchResultDto
    {
        public string? Query { get; set; }
        public string? TargetUrl { get; set; }
        public string? SearchEngine { get; set; }
        public List<int>? Rankings { get; set; }
        public DateTime SearchDate { get; set; }
    }
}