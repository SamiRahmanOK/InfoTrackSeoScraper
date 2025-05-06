using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using InfoTrackSeoScraper.Main.Application.Services;
using InfoTrackSeoScraper.Main.Core.Interfaces;
using InfoTrackSeoScraper.Main.Core.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InfoTrackSeoScraper.Tests.Services
{
    public class RankingServiceTests
    {
        private readonly Mock<ISearchResultRepository> _mockRepository;
        private readonly Mock<ILogger<RankingService>> _mockLogger;
        private readonly RankingService _service;

        public RankingServiceTests()
        {
            _mockRepository = new Mock<ISearchResultRepository>();
            _mockLogger = new Mock<ILogger<RankingService>>();
            _service = new RankingService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public void ProcessHtmlContent_WithValidContent_ReturnsRankings()
        {
            // Arrange
            var htmlContent = @"
                <html>
                <body>
                    <li class='b_algo'>
                        <a href='https://www.example.com'>Example</a>
                    </li>
                    <li class='b_algo'>
                        <a href='https://www.infotrack.co.uk/page'>InfoTrack</a>
                    </li>
                    <li class='b_algo'>
                        <a href='https://www.somesite.com'>Some site</a>
                    </li>
                </body>
                </html>";
            
            var targetUrl = "infotrack.co.uk";

            // Act
            var result = _service.ProcessHtmlContent(htmlContent, targetUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(2, result[0]);
        }

        [Fact]
        public void ProcessHtmlContent_WithNoMatches_ReturnsZero()
        {
            // Arrange
            var htmlContent = @"
                <html>
                <body>
                    <li class='b_algo'>
                        <a href='https://www.example.com'>Example</a>
                    </li>
                    <li class='b_algo'>
                        <a href='https://www.somesite.com'>Some site</a>
                    </li>
                </body>
                </html>";
            
            var targetUrl = "infotrack.co.uk";

            // Act
            var result = _service.ProcessHtmlContent(htmlContent, targetUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(0, result[0]);
        }

        [Fact]
        public void ProcessHtmlContent_WithEmptyContent_ThrowsArgumentException()
        {
            // Arrange
            var htmlContent = "";
            var targetUrl = "infotrack.co.uk";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _service.ProcessHtmlContent(htmlContent, targetUrl));
            
            Assert.Equal("HTML content cannot be empty (Parameter 'htmlContent')", exception.Message);
        }

        [Fact]
        public void ProcessHtmlContent_WithEmptyTargetUrl_ThrowsArgumentException()
        {
            // Arrange
            var htmlContent = "<html><body>Test content</body></html>";
            var targetUrl = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _service.ProcessHtmlContent(htmlContent, targetUrl));
            
            Assert.Equal("Target URL cannot be empty (Parameter 'targetUrl')", exception.Message);
        }

        [Fact]
        public async Task SaveResultAsync_SavesSearchResult()
        {
            // Arrange
            string query = "test query";
            string targetUrl = "example.com";
            string searchEngine = "bing";
            List<int> rankings = new List<int> { 1, 5 };

            _mockRepository.Setup(r => r.SaveAsync(It.IsAny<SearchResult>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.SaveResultAsync(query, targetUrl, searchEngine, rankings);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(query, result.Query);
            Assert.Equal(targetUrl, result.TargetUrl);
            Assert.Equal(searchEngine, result.SearchEngine);
            Assert.Equal(rankings, result.Rankings);

            _mockRepository.Verify(r => r.SaveAsync(It.IsAny<SearchResult>()), Times.Once);
        }

        [Fact]
        public async Task SaveResultAsync_WithEmptyQuery_ThrowsInvalidOperationException()
        {
            // Arrange
            string query = "";
            string targetUrl = "example.com";
            string searchEngine = "bing";
            List<int> rankings = new List<int> { 1, 5 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.SaveResultAsync(query, targetUrl, searchEngine, rankings));
            
            // Check inner exception is the expected ArgumentException
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Query cannot be empty (Parameter 'query')", exception.InnerException.Message);
        }

        [Fact]
        public async Task GetAllResultsAsync_ReturnsAllResults()
        {
            // Arrange
            var searchResults = new List<SearchResult>
            {
                new SearchResult
                {
                    Query = "test query",
                    TargetUrl = "example.com",
                    SearchEngine = "bing",
                    Rankings = "1, 5",
                    SearchDate = DateTime.UtcNow
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(searchResults);

            // Act
            var results = await _service.GetAllResultsAsync();

            // Assert
            Assert.NotNull(results);
            Assert.Single(results);
            
            var firstResult = results.First();
            Assert.Equal("test query", firstResult.Query);
            Assert.Equal("example.com", firstResult.TargetUrl);
            Assert.Equal("bing", firstResult.SearchEngine);
            Assert.Equal(new List<int> { 1, 5 }, firstResult.Rankings);
        }

        [Fact]
        public async Task GetAllResultsAsync_WithEmptyRepository_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<SearchResult>());

            // Act
            var results = await _service.GetAllResultsAsync();

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }
    }
}