using System;
using System.Collections.Generic;
using InfoTrackSeoScraper.Main.Application.Services;
using InfoTrackSeoScraper.Main.Core.Enums;
using InfoTrackSeoScraper.Main.Core.Interfaces;
using Moq;
using Xunit;

namespace InfoTrackSeoScraper.Tests.Services
{
    public class SearchEngineFactoryTests
    {
        private readonly Mock<IServiceProviderAdapter> _mockServiceAdapter;
        private readonly Mock<ISearchEngineScraperService> _mockGoogleScraper;
        private readonly Mock<ISearchEngineScraperService> _mockBingScraper;
        private readonly SearchEngineFactory _factory;

        public SearchEngineFactoryTests()
        {
            _mockServiceAdapter = new Mock<IServiceProviderAdapter>();
            _mockGoogleScraper = new Mock<ISearchEngineScraperService>();
            _mockBingScraper = new Mock<ISearchEngineScraperService>();
            
            _mockServiceAdapter
                .Setup(s => s.GetScraperService(SearchEngineType.Google))
                .Returns(_mockGoogleScraper.Object);
                
            _mockServiceAdapter
                .Setup(s => s.GetScraperService(SearchEngineType.Bing))
                .Returns(_mockBingScraper.Object);
                
            _factory = new SearchEngineFactory(_mockServiceAdapter.Object);
        }

        [Fact]
        public void GetScraper_WithGoogleEngine_ReturnsGoogleScraper()
        {
            // Act
            var scraper = _factory.GetScraper("google");

            // Assert
            Assert.Same(_mockGoogleScraper.Object, scraper);
            _mockServiceAdapter.Verify(sp => 
                sp.GetScraperService(SearchEngineType.Google), Times.Once);
        }

        [Fact]
        public void GetScraper_WithBingEngine_ReturnsBingScraper()
        {
            // Act
            var scraper = _factory.GetScraper("bing");

            // Assert
            Assert.Same(_mockBingScraper.Object, scraper);
            _mockServiceAdapter.Verify(sp => 
                sp.GetScraperService(SearchEngineType.Bing), Times.Once);
        }

        [Fact]
        public void GetScraper_WithCaseInsensitiveEngine_ReturnsCorrectScraper()
        {
            // Act
            var scraper = _factory.GetScraper("BiNg");

            // Assert
            Assert.Same(_mockBingScraper.Object, scraper);
            _mockServiceAdapter.Verify(sp => 
                sp.GetScraperService(SearchEngineType.Bing), Times.Once);
        }

        [Fact]
        public void GetScraper_WithUnknownEngine_DefaultsToBing()
        {
            // Act
            var scraper = _factory.GetScraper("unknown");

            // Assert
            Assert.Same(_mockBingScraper.Object, scraper);
            _mockServiceAdapter.Verify(sp => 
                sp.GetScraperService(SearchEngineType.Bing), Times.Once);
        }

        [Fact]
        public void GetScraper_WithEmptyString_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _factory.GetScraper(""));
            Assert.Equal("Engine name cannot be empty (Parameter 'engineName')", exception.Message);
        }

        [Fact]
        public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new SearchEngineFactory(null));
            Assert.Equal("Value cannot be null. (Parameter 'serviceProviderAdapter')", exception.Message);
        }
    }
}