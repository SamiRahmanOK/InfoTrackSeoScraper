using InfoTrackSeoScraper.Main.Application.DTOs;

namespace InfoTrackSeoScraper.Main.Core.Interfaces
{
    public interface IRankingService
    {
        List<int> ProcessHtmlContent(string htmlContent, string targetUrl);
        Task<SearchResultDto> SaveResultAsync(string query, string targetUrl, string searchEngine, List<int> rankings);
        Task<IEnumerable<SearchResultDto>> GetAllResultsAsync();
    }
}