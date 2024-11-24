using Microsoft.EntityFrameworkCore;
using System.Linq;
using Wise_Owl_Library.Models;
using Wise_Owl_Library.Extensions;
using Wise_Owl_Library.Models.Entities;

namespace Wise_Owl_Library.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooksAsync(string? title, int? stock);
        Task<Book> GetBookAsync(int id);
        Task<List<Book>> AddBooksAsync(List<Book> books);
        Task<Book> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<bool> BookExistsAsync(string title, List<Author> authorNames);
        Task<bool> BookExistsByIdAsync(int id);
    }
    public class BookSqlRepository(WiseOwlLibraryDbContext wiseOwlLibraryDbContext) : IBookRepository
    {
        public async Task<List<Book>> AddBooksAsync(List<Book> books)
        {
            List<Book> createdBooks = new();
            foreach (Book book in books)
            {
                if(await wiseOwlLibraryDbContext.Books.AddAsync(book.ToBookEntity()) != null)
                {
                    createdBooks.Add(book);
                }
            }
            await wiseOwlLibraryDbContext.SaveChangesAsync();

            return createdBooks;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            BookEntity? book = await wiseOwlLibraryDbContext.Books.FindAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with Id {id} not found.");
            }
            if(wiseOwlLibraryDbContext.Books.Remove(book) != null)
            {
                await wiseOwlLibraryDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Book> GetBookAsync(int id)
        {
            BookEntity? book = await wiseOwlLibraryDbContext.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with Id {id} not found.");
            }
            return book.EntityToBook();
        }

        public async Task<List<Book>> GetBooksAsync(string? title, int? stock)
        {
            IQueryable<BookEntity> query = wiseOwlLibraryDbContext.Books.Include(b => b.Authors);

            if (title != null)
            {
                query = query.Where(b => b.Title == title);
            }

            if (stock != null)
            {
                query = query.Where(b => b.Stock == stock);
            }

            List<BookEntity> books = await query.ToListAsync();
            return books.Select(b => b.EntityToBook()).ToList();
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            BookEntity? bookEntity = await wiseOwlLibraryDbContext.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == book.Id);

            if (bookEntity == null)
            {
                throw new KeyNotFoundException($"Book with Id {book.Id} not found.");
            }

            bookEntity.Title = book.Title;
            bookEntity.Price = book.Price;
            bookEntity.Stock = book.Stock;
            bookEntity.Authors = book.Authors.Select(a => new AuthorEntity { Id = a.Id, Name = a.Name }).ToList();

            wiseOwlLibraryDbContext.Entry(bookEntity).State = EntityState.Modified;
            await wiseOwlLibraryDbContext.SaveChangesAsync();

            return bookEntity.EntityToBook();
        }

        public async Task<bool> BookExistsAsync(string title, List<Author> authors)
        {
            if (await wiseOwlLibraryDbContext.Books.AnyAsync(b => b.Title == title && b.Authors.SequenceEqual(authors.Select(a => a.ToAuthorEntity()))))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> BookExistsByIdAsync(int id)
        {
            if (await wiseOwlLibraryDbContext.Books.AnyAsync(b => b.Id == id))
            {
                return true;
            }
            return false;
        }
    }
}
