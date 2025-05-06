using InfoTrackSeoScraper.Main.Core.Interfaces;
using InfoTrackSeoScraper.Main.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace InfoTrackSeoScraper.Main.Application.Services
{
    /// <summary>
    /// Factory for creating appropriate search engine scraper services.
    /// </summary>
    public class SearchEngineFactory : ISearchEngineFactory
    {
        private readonly IServiceProviderAdapter _serviceProviderAdapter;
        private readonly Dictionary<string, SearchEngineType> _engineMap;

        public SearchEngineFactory(IServiceProviderAdapter serviceProviderAdapter)
        {
            _serviceProviderAdapter = serviceProviderAdapter ?? throw new ArgumentNullException(nameof(serviceProviderAdapter));
            
            // Map engine names to enum values (case-insensitive)
            _engineMap = new Dictionary<string, SearchEngineType>(StringComparer.OrdinalIgnoreCase)
            {
                { "google", SearchEngineType.Google },
                { "bing", SearchEngineType.Bing }
            };
        }

        /// <summary>
        /// Gets a search engine scraper for the specified engine, defaulting to Bing if not found.
        /// </summary>
        public virtual ISearchEngineScraperService GetScraper(string engineName)
        {
            if (string.IsNullOrWhiteSpace(engineName))
                throw new ArgumentException("Engine name cannot be empty", nameof(engineName));

            // Try to get the engine type, default to Bing if not found
            if (!_engineMap.TryGetValue(engineName, out var engineType))
                engineType = SearchEngineType.Bing;

            return _serviceProviderAdapter.GetScraperService(engineType);
        }
    }
}