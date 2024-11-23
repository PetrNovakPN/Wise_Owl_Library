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
            BookEntity? book = await wiseOwlLibraryDbContext.Books.FindAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with Id {id} not found.");
            }
            return book.EntityToBook();
        }

        public async Task<List<Book>> GetBooksAsync(string? title, int? stock)
        {
            List<BookEntity> books = new();
            if (title != null)
            {
                books = await wiseOwlLibraryDbContext.Books.Where(b => b.Title == title).ToListAsync();
            }
            else if (stock != null)
            {
                books = await wiseOwlLibraryDbContext.Books.Where(b => b.Stock == stock).ToListAsync();
            }
            else
            {
                books = await wiseOwlLibraryDbContext.Books.ToListAsync();
            }
            return books.Select(b => b.EntityToBook()).ToList();
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            BookEntity updatedBook = wiseOwlLibraryDbContext.Books.Update(book.ToBookEntity()).Entity;
            await wiseOwlLibraryDbContext.SaveChangesAsync();

            return updatedBook.EntityToBook();
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
