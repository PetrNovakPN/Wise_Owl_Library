using Wise_Owl_Library.Models;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Models.Entities;

namespace Wise_Owl_Library.Repositories
{
    public class WiseOwlLibraryDbContext(DbContextOptions<WiseOwlLibraryDbContext> options) : DbContext(options)
    {
        public required DbSet<AuthorEntity> Authors { get; set; }
        public required DbSet<BookEntity> Books { get; set; }
        public required DbSet<PriceChangeEntity> PriceChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookEntity>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books);

            base.OnModelCreating(modelBuilder);
        }
    }
}