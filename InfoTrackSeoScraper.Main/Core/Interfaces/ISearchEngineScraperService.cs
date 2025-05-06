namespace InfoTrackSeoScraper.Main.Core.Interfaces
{
    public interface ISearchEngineScraperService
    {
        Task<string> ScrapeSearchResultsAsync(string query);
    }
}