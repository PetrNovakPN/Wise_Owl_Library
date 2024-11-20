using Wise_Owl_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Wise_Owl_Library.Repositories
{
    public class WiseOwlLibraryDbContext(DbContextOptions<WiseOwlLibraryDbContext> options) : DbContext(options)
    {
        public required DbSet<Author> Authors { get; set; }
        public required DbSet<Book> Books { get; set; }
        public required DbSet<PriceChange> PriceChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books);

            base.OnModelCreating(modelBuilder);
        }
    }
}