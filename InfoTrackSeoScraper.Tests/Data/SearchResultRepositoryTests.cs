using System;
using System.Linq;
using System.Threading.Tasks;
using InfoTrackSeoScraper.Main.Core.Entities;
using InfoTrackSeoScraper.Main.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InfoTrackSeoScraper.Tests.Data
{
    public class SearchResultRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ILogger<SearchResultRepository>> _mockLogger;

        public SearchResultRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
                
            _mockLogger = new Mock<ILogger<SearchResultRepository>>();
        }

        [Fact]
        public async Task SaveAsync_AddsEntityToDatabase()
        {
            // Arrange
            var searchResult = new SearchResult
            {
                Query = "test query",
                TargetUrl = "example.com",
                SearchEngine = "bing",
                Rankings = "1, 5",
                SearchDate = DateTime.UtcNow
            };

            // Act
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new SearchResultRepository(context, _mockLogger.Object);
                await repository.SaveAsync(searchResult);
            }

            // Assert
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                Assert.Equal(1, await context.SearchResults.CountAsync());
                
                var savedResult = await context.SearchResults.FirstOrDefaultAsync();
                Assert.NotNull(savedResult);
                Assert.Equal("test query", savedResult.Query);
                Assert.Equal("example.com", savedResult.TargetUrl);
                Assert.Equal("bing", savedResult.SearchEngine);
                Assert.Equal("1, 5", savedResult.Rankings);
            }
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntitiesOrderedByDate()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.SearchResults.Add(new SearchResult
                {
                    Query = "test query 1",
                    TargetUrl = "example1.com",
                    SearchEngine = "bing",
                    Rankings = "1, 5",
                    SearchDate = DateTime.UtcNow.AddDays(-1)
                });
                
                context.SearchResults.Add(new SearchResult
                {
                    Query = "test query 2",
                    TargetUrl = "example2.com",
                    SearchEngine = "google",
                    Rankings = "3",
                    SearchDate = DateTime.UtcNow
                });
                
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new SearchResultRepository(context, _mockLogger.Object);
                var results = (await repository.GetAllAsync()).ToList();

                // Assert
                Assert.Equal(2, results.Count);
                
                // Results should be ordered by date descending, so newest first
                Assert.Equal("test query 2", results[0].Query);
                Assert.Equal("test query 1", results[1].Query);
            }
        }

        [Fact]
        public void Constructor_WithNullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => 
                new SearchResultRepository(null, _mockLogger.Object));
                
            Assert.Equal("Value cannot be null. (Parameter 'context')", exception.Message);
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var context = new ApplicationDbContext(_dbContextOptions);
            
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => 
                new SearchResultRepository(context, null));
                
            Assert.Equal("Value cannot be null. (Parameter 'logger')", exception.Message);
        }
    }
}