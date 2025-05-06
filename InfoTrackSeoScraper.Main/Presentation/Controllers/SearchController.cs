using Microsoft.AspNetCore.Mvc;
using InfoTrackSeoScraper.Main.Application.Services;
using InfoTrackSeoScraper.Main.Application.DTOs;
using InfoTrackSeoScraper.Main.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace InfoTrackSeoScraper.Main.Presentation.Controllers
{
    /// <summary>
    /// API controller for handling search-related operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IRankingService _rankingService;
        private readonly ISearchEngineFactory _searchEngineFactory;
        private readonly ILogger<SearchController> _logger;

        public SearchController(
            IRankingService rankingService,
            ISearchEngineFactory searchEngineFactory,
            ILogger<SearchController> logger)
        {
            _rankingService = rankingService ?? throw new ArgumentNullException(nameof(rankingService));
            _searchEngineFactory = searchEngineFactory ?? throw new ArgumentNullException(nameof(searchEngineFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Searches for a target URL in search engine results and returns the rankings.
        /// </summary>
        /// <response code="200">Returns the search result with rankings.</response>
        /// <response code="400">If the query parameters are invalid.</response>
        /// <response code="500">If an error occurs during processing.</response>
        [HttpGet]
        [ProducesResponseType(typeof(SearchResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(
            [Required][FromQuery] string query, 
            [Required][FromQuery] string targetUrl, 
            [Required][FromQuery] string engine)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(targetUrl) || string.IsNullOrWhiteSpace(engine))
            {
                return BadRequest(new { error = "Query, target URL, and search engine are required." });
            }

            try
            {
                // Validate the URL format
                if (!IsValidUrl(targetUrl))
                {
                    return BadRequest(new { error = "Invalid target URL format." });
                }

                // Use factory to get the appropriate scraper service
                var scraperService = _searchEngineFactory.GetScraper(engine);
                
                if (scraperService == null)
                {
                    return BadRequest(new { error = $"Unsupported search engine: {engine}" });
                }

                string htmlContent = await scraperService.ScrapeSearchResultsAsync(query);
                
                var rankings = _rankingService.ProcessHtmlContent(htmlContent, targetUrl);
                var result = await _rankingService.SaveResultAsync(query, targetUrl, engine, rankings);

                // Return a standardized response structure
                var response = new SearchResponseDto
                {
                    Rankings = rankings,
                    Query = query,
                    TargetUrl = targetUrl,
                    SearchEngine = engine
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing search request for {Engine}", engine);
                return StatusCode(500, new { error = "An unexpected error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Retrieves the search history from the database.
        /// </summary>
        /// <response code="200">Returns the search history.</response>
        /// <response code="500">If an error occurs during retrieval.</response>
        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSearchHistory()
        {
            try
            {
                var searchHistory = await _rankingService.GetAllResultsAsync();
                return Ok(searchHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching search history");
                return StatusCode(500, new { error = "An unexpected error occurred while retrieving search history." });
            }
        }

        /// <summary>
        /// Validates if a string is a valid URL.
        /// </summary>
        /// <remarks>
        /// Adds "https://" protocol if not present before validation.
        /// </remarks>
        private bool IsValidUrl(string url)
        {
            try
            {
                // Add "https://" if the URL doesn't have a protocol
                var normalizedUrl = url.StartsWith("http://") || url.StartsWith("https://") ? url : $"https://{url}";
                var parsedUrl = new Uri(normalizedUrl);
                return !string.IsNullOrWhiteSpace(parsedUrl.Host);
            }
            catch
            {
                return false;
            }
        }
    }
}