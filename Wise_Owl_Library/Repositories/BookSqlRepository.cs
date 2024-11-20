using Microsoft.EntityFrameworkCore;
using System.Linq;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooksAsync(string? title, int? stock);
        Task<Book> GetBookAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<bool> BookExistsAsync(string title, List<Author> authorNames);
        Task<bool> BookExistsByIdAsync(int id);
    }
    public class BookSqlRepository(WiseOwlLibraryDbContext wiseOwlLibraryDbContext) : IBookRepository
    {
        public Task<Book> AddBookAsync(Book book)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBookAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Book> GetBookAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Book>> GetBooksAsync(string? title, int? stock)
        {

            throw new NotImplementedException();
        }

        public Task<Book> UpdateBookAsync(Book book)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> BookExistsAsync(string title, List<Author> authors)
        {
            if (await wiseOwlLibraryDbContext.Books.AnyAsync(b => b.Title == title && b.Authors.SequenceEqual(authors)))
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
