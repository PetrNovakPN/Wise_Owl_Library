using Wise_Owl_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Wise_Owl_Library.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {

        // DbSet pro entity
        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }

        public DbSet<PriceChange> PriceChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Nastavení primárního klíce pro entitu BookAuthor
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            // Nastavení vztahu mezi BookAuthor a Book
            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(ba => ba.BookId);

            // Nastavení vztahu mezi BookAuthor a Author
            modelBuilder.Entity<BookAuthor>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(ba => ba.AuthorId);

            // Seed dat pro entity
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Harry Potter a Kámen mudrců", Price = 399, Stock = 10 },
                new Book { Id = 2, Title = "Harry Potter a Tajemná komnata", Price = 399, Stock = 10 },
                new Book { Id = 3, Title = "Pán prstenů: Spolecenstvo prstenu", Price = 499, Stock = 5 },
                new Book { Id = 4, Title = "Hobit", Price = 299, Stock = 8 },
                new Book { Id = 5, Title = "Hra o trůny", Price = 599, Stock = 3 }
            );

            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "J. K. Rowling" },
                new Author { Id = 2, Name = "J. R. R. Tolkien" },
                new Author { Id = 3, Name = "George R. R. Martin" }
            );

            modelBuilder.Entity<BookAuthor>().HasData(
                new BookAuthor { BookId = 1, AuthorId = 1 },
                new BookAuthor { BookId = 2, AuthorId = 1 },
                new BookAuthor { BookId = 3, AuthorId = 2 },
                new BookAuthor { BookId = 4, AuthorId = 2 },
                new BookAuthor { BookId = 5, AuthorId = 3 }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
