using Wise_Owl_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Wise_Owl_Library.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // DbSet pro entity
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<PriceChange> PriceChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Nastavení many-to-many vztahu mezi Book a Author
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books);


            base.OnModelCreating(modelBuilder);
        }
    }
}
