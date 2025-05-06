namespace InfoTrackSeoScraper.Main.Application.DTOs
{
    public class SearchResponseDto
    {
        public List<int> Rankings { get; set; } = new List<int>();
        public required string Query { get; set; }
        public required string TargetUrl { get; set; }
        public required string SearchEngine { get; set; }
    }
}