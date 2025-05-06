namespace InfoTrackSeoScraper.Main.Core.Interfaces
{
    public interface ISearchEngineFactory
    {
        ISearchEngineScraperService GetScraper(string engineName);
    }
}