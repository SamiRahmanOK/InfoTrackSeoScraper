using InfoTrackSeoScraper.Main.Core.Entities;
using InfoTrackSeoScraper.Main.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace InfoTrackSeoScraper.Main.Infrastructure.Data
{
    /// <summary>
    /// Repository for managing search result data persistence.
    /// </summary>
    public class SearchResultRepository : ISearchResultRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SearchResultRepository> _logger;

        public SearchResultRepository(ApplicationDbContext context, ILogger<SearchResultRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all search results ordered by most recent first.
        /// </summary>
        public async Task<IEnumerable<SearchResult>> GetAllAsync()
        {
            try
            {
                return await _context.SearchResults
                    .OrderByDescending(sr => sr.SearchDate)
                    .ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while retrieving search results");
                throw new InvalidOperationException("An error occurred while retrieving data from the database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving search results");
                throw new IOException("An unexpected error occurred while accessing the database", ex);
            }
        }

        /// <summary>
        /// Saves a search result to the database.
        /// </summary>
        public async Task SaveAsync(SearchResult searchResult)
        {
            if (searchResult == null)
            {
                throw new ArgumentNullException(nameof(searchResult), "Search result cannot be null");
            }

            try
            {
                _context.SearchResults.Add(searchResult);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving search result: {Query}, {TargetUrl}", 
                    searchResult.Query, searchResult.TargetUrl);
                throw new InvalidOperationException("Failed to save search result to the database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error saving search result");
                throw new IOException("An unexpected error occurred while saving to the database", ex);
            }
        }
    }
}