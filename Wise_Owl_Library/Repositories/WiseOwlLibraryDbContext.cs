using Wise_Owl_Library.Models;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Models.Entities;
using Wise_Owl_Library.Repositories.Interceptors;

namespace Wise_Owl_Library.Repositories
{
    public class WiseOwlLibraryDbContext(DbContextOptions<WiseOwlLibraryDbContext> options, PriceChangeInterceptor priceChangeInterceptor) : DbContext(options)
    {
        public required DbSet<AuthorEntity> Authors { get; set; }
        public required DbSet<BookEntity> Books { get; set; }
        public required DbSet<PriceChangeEntity> PriceChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookEntity>()
                .HasMany(b => b.Authors);

            modelBuilder.Entity<AuthorEntity>()
                .HasMany(a => a.Books);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(priceChangeInterceptor);
            base.OnConfiguring(optionsBuilder);
        }
    }
}