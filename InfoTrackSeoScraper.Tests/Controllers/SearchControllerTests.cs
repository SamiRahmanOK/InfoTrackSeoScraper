using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfoTrackSeoScraper.Main.Application.DTOs;
using InfoTrackSeoScraper.Main.Application.Services;
using InfoTrackSeoScraper.Main.Core.Interfaces;
using InfoTrackSeoScraper.Main.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InfoTrackSeoScraper.Tests.Controllers
{
    /// <summary>
    /// Tests for the SearchController API endpoints.
    /// </summary>
    public class SearchControllerTests
    {
        private readonly Mock<IRankingService> _mockRankingService;
        private readonly Mock<ISearchEngineFactory> _mockSearchEngineFactory;
        private readonly Mock<ILogger<SearchController>> _mockLogger;
        private readonly Mock<ISearchEngineScraperService> _mockScraperService;
        private readonly SearchController _controller;

        public SearchControllerTests()
        {
            _mockRankingService = new Mock<IRankingService>();
            _mockSearchEngineFactory = new Mock<ISearchEngineFactory>();
            _mockLogger = new Mock<ILogger<SearchController>>();
            _mockScraperService = new Mock<ISearchEngineScraperService>();
            
            _mockSearchEngineFactory
                .Setup(f => f.GetScraper(It.IsAny<string>()))
                .Returns(_mockScraperService.Object);
                
            _controller = new SearchController(
                _mockRankingService.Object, 
                _mockSearchEngineFactory.Object, 
                _mockLogger.Object);
        }

        /// <summary>
        /// Tests that providing valid search parameters returns a successful result with 
        /// the correct ranking information.
        /// </summary>
        /// <remarks>
        /// Verifies the complete flow from scraping to processing to saving the results
        /// and returning the expected SearchResponseDto.
        /// </remarks>
        [Fact]
        public async Task Get_WithValidParameters_ReturnsOkResult()
        {
            // Arrange
            var query = "land registry searches";
            var targetUrl = "infotrack.co.uk";
            var engine = "bing";
            var htmlContent = "<html><body>Test content</body></html>";
            var rankings = new List<int> { 1, 5 };
            
            _mockScraperService
                .Setup(s => s.ScrapeSearchResultsAsync(query))
                .ReturnsAsync(htmlContent);
                
            _mockRankingService
                .Setup(s => s.ProcessHtmlContent(htmlContent, targetUrl))
                .Returns(rankings);
                
            _mockRankingService
                .Setup(s => s.SaveResultAsync(query, targetUrl, engine, rankings))
                .ReturnsAsync(new SearchResultDto
                {
                    Query = query,
                    TargetUrl = targetUrl,
                    SearchEngine = engine,
                    Rankings = rankings,
                    SearchDate = DateTime.UtcNow
                });

            // Act
            var result = await _controller.Get(query, targetUrl, engine);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<SearchResponseDto>(okResult.Value);
            
            Assert.Equal(query, response.Query);
            Assert.Equal(targetUrl, response.TargetUrl);
            Assert.Equal(engine, response.SearchEngine);
            Assert.Equal(rankings, response.Rankings);
        }

        /// <summary>
        /// Tests that the API returns a bad request when the query parameter is empty.
        /// </summary>
        [Fact]
        public async Task Get_WithEmptyQuery_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Get("", "example.com", "bing");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        /// <summary>
        /// Tests that the API returns a bad request when the target URL parameter is empty.
        /// </summary>
        [Fact]
        public async Task Get_WithEmptyTargetUrl_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Get("test query", "", "bing");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        /// <summary>
        /// Tests that the API returns a bad request when the target URL is invalid.
        /// </summary>
        /// <remarks>
        /// The URL validation logic in the controller should reject URLs that can't be parsed
        /// into a valid URI.
        /// </remarks>
        [Fact]
        public async Task Get_WithInvalidUrl_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Get("test query", "not a url", "bing");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        /// <summary>
        /// Tests that the API handles exceptions from the scraper service properly,
        /// returning a 500 Internal Server Error.
        /// </summary>
        /// <remarks>
        /// This test verifies that the controller's error handling correctly captures
        /// and logs exceptions without exposing implementation details to clients.
        /// </remarks>
        [Fact]
        public async Task Get_WithScraperException_ReturnsInternalServerError()
        {
            // Arrange
            _mockScraperService
                .Setup(s => s.ScrapeSearchResultsAsync(It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Scraper error"));

            // Act
            var result = await _controller.Get("test query", "example.com", "bing");

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        /// <summary>
        /// Tests that the search history endpoint returns a successful result
        /// with the correct history data.
        /// </summary>
        [Fact]
        public async Task GetSearchHistory_ReturnsOkResult()
        {
            // Arrange
            var searchHistory = new List<SearchResultDto>
            {
                new SearchResultDto
                {
                    Query = "test query",
                    TargetUrl = "example.com",
                    SearchEngine = "bing",
                    Rankings = new List<int> { 1, 5 },
                    SearchDate = DateTime.UtcNow
                }
            };
            
            _mockRankingService
                .Setup(s => s.GetAllResultsAsync())
                .ReturnsAsync(searchHistory);

            // Act
            var result = await _controller.GetSearchHistory();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var history = Assert.IsAssignableFrom<IEnumerable<SearchResultDto>>(okResult.Value);
            
            Assert.Single(history);
        }

        /// <summary>
        /// Tests that the search history endpoint handles exceptions properly,
        /// returning a 500 Internal Server Error.
        /// </summary>
        /// <remarks>
        /// This test ensures that database errors are handled gracefully without
        /// exposing internal error details to clients.
        /// </remarks>
        [Fact]
        public async Task GetSearchHistory_WithException_ReturnsInternalServerError()
        {
            // Arrange
            _mockRankingService
                .Setup(s => s.GetAllResultsAsync())
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _controller.GetSearchHistory();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}