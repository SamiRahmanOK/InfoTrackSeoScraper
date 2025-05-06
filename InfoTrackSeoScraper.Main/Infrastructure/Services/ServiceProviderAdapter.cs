using InfoTrackSeoScraper.Main.Core.Enums;
using InfoTrackSeoScraper.Main.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace InfoTrackSeoScraper.Main.Infrastructure.Services
{
    /// <summary>
    /// Adapter for resolving search engine scraper services from the DI container.
    /// </summary>
    public class ServiceProviderAdapter : IServiceProviderAdapter
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderAdapter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Gets the appropriate scraper service for the specified search engine type.
        /// </summary>
        /// <remarks>
        /// Uses keyed services to resolve the correct implementation based on engine type.
        /// </remarks>
        public ISearchEngineScraperService GetScraperService(SearchEngineType engineType)
        {
            return _serviceProvider.GetKeyedService<ISearchEngineScraperService>(engineType) 
                ?? throw new InvalidOperationException($"No service registered for engine type: {engineType}");
        }
    }
}