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


            //// Seed dat pro entity
            //var author1 = new Author { Id = 1, Name = "J. K. Rowling" };
            //var author2 = new Author { Id = 2, Name = "J. R. R. Tolkien" };
            //var author3 = new Author { Id = 3, Name = "George R. R. Martin" };

            //var book1 = new Book { Id = 1, Title = "Harry Potter a Kámen mudrců", Price = 399, Stock = 10, Authors = [author1] };
            //var book2 = new Book { Id = 2, Title = "Harry Potter a Tajemná komnata", Price = 399, Stock = 10, Authors = [author1] };
            //var book3 = new Book { Id = 3, Title = "Pán prstenů: Spolecenstvo prstenu", Price = 499, Stock = 5, Authors = [author2] };
            //var book4 = new Book { Id = 4, Title = "Hobit", Price = 299, Stock = 8, Authors = [author2] };
            //var book5 = new Book { Id = 5, Title = "Hra o trůny", Price = 599, Stock = 3, Authors = [author3] };

            //modelBuilder.Entity<Author>().HasData(author1, author2, author3);
            //modelBuilder.Entity<Book>().HasData(book1, book2, book3, book4, book5);
            

            base.OnModelCreating(modelBuilder);
        }
    }
}
