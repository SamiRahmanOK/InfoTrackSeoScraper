using InfoTrackSeoScraper.Main.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfoTrackSeoScraper.Main.Infrastructure.Services
{
    /// <summary>
    /// Service for scraping search results from Google search engine.
    /// </summary>
    public class GoogleScraperService : ISearchEngineScraperService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleScraperService> _logger;

        public GoogleScraperService(HttpClient httpClient, ILogger<GoogleScraperService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves search results from Google for the provided query.
        /// </summary>
        /// <remarks>
        /// Requests 100 results to provide comprehensive ranking analysis.
        /// </remarks>
        public async Task<string> ScrapeSearchResultsAsync(string query)
        {
            try
            {
                var encodedQuery = Uri.EscapeDataString(query);
                var url = $"https://www.google.co.uk/search?num=100&q={encodedQuery}";
                
                // Add headers to mimic a browser request
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                
                var response = await _httpClient.GetStringAsync(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping Google search results for query: {Query}", query);
                throw;
            }
        }
    }
}