using InfoTrackSeoScraper.Main.Core.Enums;
using InfoTrackSeoScraper.Main.Core.Interfaces;

namespace InfoTrackSeoScraper.Main.Core.Interfaces
{
    public interface IServiceProviderAdapter
    {
        ISearchEngineScraperService GetScraperService(SearchEngineType engineType);
    }
}