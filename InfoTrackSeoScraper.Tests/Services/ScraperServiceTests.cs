using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using InfoTrackSeoScraper.Main.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace InfoTrackSeoScraper.Tests.Services
{
    public class BingScraperServiceTests
    {
        private readonly Mock<ILogger<BingScraperService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;

        public BingScraperServiceTests()
        {
            _mockLogger = new Mock<ILogger<BingScraperService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://www.bing.com")
            };
        }

        [Fact]
        public async Task ScrapeSearchResultsAsync_ReturnsHtmlContent()
        {
            // Arrange
            var query = "land registry searches";
            var expectedHtml = "<html><body>Test content</body></html>";
            
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedHtml)
                });
            
            var scraperService = new BingScraperService(_httpClient, _mockLogger.Object);

            // Act
            var result = await scraperService.ScrapeSearchResultsAsync(query);

            // Assert
            Assert.Equal(expectedHtml, result);
            
            _mockHttpMessageHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri.ToString().StartsWith($"https://www.bing.com/search?q={query}&count=100")),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task ScrapeSearchResultsAsync_WithHttpError_ThrowsHttpRequestException()
        {
            // Arrange
            var query = "land registry searches";
            
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });
            
            var scraperService = new BingScraperService(_httpClient, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                scraperService.ScrapeSearchResultsAsync(query));
        }

        [Fact]
        public async Task ScrapeSearchResultsAsync_WithEmptyQuery_ThrowsInvalidOperationException()
        {
            // Arrange
            var scraperService = new BingScraperService(_httpClient, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                scraperService.ScrapeSearchResultsAsync(""));
        }
    }

    public class GoogleScraperServiceTests
    {
        private readonly Mock<ILogger<GoogleScraperService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;

        public GoogleScraperServiceTests()
        {
            _mockLogger = new Mock<ILogger<GoogleScraperService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://www.google.co.uk")
            };
        }

        [Fact]
        public async Task ScrapeSearchResultsAsync_ReturnsHtmlContent()
        {
            // Arrange
            var query = "land registry searches";
            var expectedHtml = "<html><body>Google results</body></html>";
            
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedHtml)
                });
            
            var scraperService = new GoogleScraperService(_httpClient, _mockLogger.Object);

            // Act
            var result = await scraperService.ScrapeSearchResultsAsync(query);

            // Assert
            Assert.Equal(expectedHtml, result);
            
            _mockHttpMessageHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri.ToString().Contains($"num=100") && 
                        req.RequestUri.ToString().Contains($"q={query}")),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
}