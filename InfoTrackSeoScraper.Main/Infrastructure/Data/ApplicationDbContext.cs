using InfoTrackSeoScraper.Main.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfoTrackSeoScraper.Main.Infrastructure.Data
{
    /// <summary>
    /// Database context for the InfoTrackSeoScraper application.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<SearchResult> SearchResults { get; set; }
    }
}