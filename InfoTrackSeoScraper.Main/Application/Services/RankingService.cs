using HtmlAgilityPack;
using InfoTrackSeoScraper.Main.Core.Entities;
using InfoTrackSeoScraper.Main.Core.Interfaces;
using InfoTrackSeoScraper.Main.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;

namespace InfoTrackSeoScraper.Main.Application.Services
{
    /// <summary>
    /// Processes HTML content from search engines to extract and manage URL rankings.
    /// </summary>
    public class RankingService : IRankingService
    {
        private readonly ISearchResultRepository _repository;
        private readonly ILogger<RankingService> _logger;

        public RankingService(ISearchResultRepository repository, ILogger<RankingService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Extracts rankings of a target URL from HTML search results.
        /// </summary>
        /// <remarks>
        /// Returns positions where target URL appears, or [0] if not found.
        /// </remarks>
        public List<int> ProcessHtmlContent(string htmlContent, string targetUrl)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
                throw new ArgumentException("HTML content cannot be empty", nameof(htmlContent));

            if (string.IsNullOrWhiteSpace(targetUrl))
                throw new ArgumentException("Target URL cannot be empty", nameof(targetUrl));

            try
            {
                var rankings = new List<int>();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                // Extract search results from the Bing result structure
                var searchResults = htmlDoc.DocumentNode.SelectNodes("//li[@class='b_algo']");
                if (searchResults == null)
                {
                    _logger.LogWarning("No search results found for target URL: {TargetUrl}", targetUrl);
                    return new List<int> { 0 };
                }

                // Find all positions where target URL appears
                int rank = 1;
                foreach (var result in searchResults)
                {
                    var linkNode = result.SelectSingleNode(".//a[@href]");
                    if (linkNode != null && linkNode.Attributes["href"].Value.Contains(targetUrl))
                    {
                        rankings.Add(rank);
                    }
                    rank++;
                }

                return rankings.Count > 0 ? rankings : new List<int> { 0 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing HTML content for target URL: {TargetUrl}", targetUrl);
                throw new InvalidOperationException($"Failed to process HTML content for target URL: {targetUrl}", ex);
            }
        }

        /// <summary>
        /// Saves search ranking results to the database.
        /// </summary>
        public async Task<SearchResultDto> SaveResultAsync(string query, string targetUrl, string searchEngine, List<int> rankings)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    throw new ArgumentException("Query cannot be empty", nameof(query));

                if (string.IsNullOrWhiteSpace(targetUrl))
                    throw new ArgumentException("Target URL cannot be empty", nameof(targetUrl));

                if (string.IsNullOrWhiteSpace(searchEngine))
                    throw new ArgumentException("Search Engine cannot be empty", nameof(searchEngine));

                if (rankings == null)
                    throw new ArgumentException("Rankings cannot be null", nameof(rankings));

                var searchResult = new SearchResult
                {
                    Query = query,
                    TargetUrl = targetUrl,
                    SearchEngine = searchEngine,
                    Rankings = string.Join(", ", rankings),
                    SearchDate = DateTime.UtcNow
                };

                await _repository.SaveAsync(searchResult);

                return new SearchResultDto
                {
                    Query = query,
                    TargetUrl = targetUrl,
                    SearchEngine = searchEngine,
                    Rankings = rankings,
                    SearchDate = searchResult.SearchDate
                };
            }
            catch (InvalidOperationException)
            {
                // Rethrow database exceptions as they're already properly handled
                throw;
            }
            catch (IOException)
            {
                // Rethrow IO exceptions as they're already properly handled
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving search result: {Query}, {TargetUrl}, {SearchEngine}", 
                    query, targetUrl, searchEngine);
                throw new InvalidOperationException("Failed to save search result", ex);
            }
        }

        /// <summary>
        /// Retrieves all historical search results.
        /// </summary>
        public async Task<IEnumerable<SearchResultDto>> GetAllResultsAsync()
        {
            try
            {
                var results = await _repository.GetAllAsync();
                
                return results.Select(r => new SearchResultDto
                {
                    Query = r.Query,
                    TargetUrl = r.TargetUrl,
                    SearchEngine = r.SearchEngine,
                    Rankings = ParseRankings(r.Rankings ?? string.Empty),
                    SearchDate = r.SearchDate
                });
            }
            catch (InvalidOperationException)
            {
                // Rethrow database exceptions as they're already properly handled
                throw;
            }
            catch (IOException)
            {
                // Rethrow IO exceptions as they're already properly handled
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all search results");
                throw new InvalidOperationException("Failed to retrieve search results", ex);
            }
        }

        // Helper method to convert comma-separated ranking string to integers
        private List<int> ParseRankings(string rankings)
        {
            if (string.IsNullOrWhiteSpace(rankings))
                return new List<int>();
                
            return rankings.Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => int.TryParse(s.Trim(), out int result) ? result : 0)
                .Where(i => i >= 0)
                .ToList();
        }
    }
}