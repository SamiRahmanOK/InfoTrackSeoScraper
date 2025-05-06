using InfoTrackSeoScraper.Main.Core.Entities;

namespace InfoTrackSeoScraper.Main.Core.Interfaces
{
    public interface ISearchResultRepository
    {
        Task<IEnumerable<SearchResult>> GetAllAsync();
        Task SaveAsync(SearchResult searchResult);
    }
}