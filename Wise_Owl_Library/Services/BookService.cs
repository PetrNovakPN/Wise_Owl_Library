using Wise_Owl_Library.Extensions;
using Wise_Owl_Library.Models;
using Wise_Owl_Library.Repositories;

namespace Wise_Owl_Library.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksAsync(string? title = null, int? stock = null);
        Task<Book?> GetBookAsync(int id);
        Task<List<Book>> CreateBooksAsync(List<Book> books);
        Task<Book?> UpdateBookAsync(Book updatedBook);
        Task<bool> DeleteBookAsync(int id);
    }

    public class BookService(IBookRepository bookRepository, ILogger<BookService> logger) : IBookService
    {
        public async Task<List<Book>> GetBooksAsync(string? title, int? stock)
        {
            return await bookRepository.GetBooksAsync(title, stock);
        }

        public async Task<Book?> GetBookAsync(int id)
        {
            Book book = await bookRepository.GetBookAsync(id);
            //TODO: udělat to jinak než posílat prázdnej novej book
            return book.Title == "" ? null : book;
        }

        public async Task<List<Book>> CreateBooksAsync(List<Book> books)
        {
            List<Book> booksToCreate = [];

            foreach (Book book in books)
            {
                //TODO: tady by možná bylo efektivnější si je poslat všechny do db layeru a až tam si je kontrolovat jestli existují, mohlo by to být méně náročný
                if (!await BookExistsAsync(book.Title, book.Authors))
                {
                    
                    booksToCreate.Add(book);
                }
            }
            List<Book> createdBooks = await bookRepository.AddBooksAsync(booksToCreate);

            return createdBooks;
        }

        public async Task<Book?> UpdateBookAsync(Book updatedBook)
        {
            Book book = await bookRepository.UpdateBookAsync(updatedBook);
            return book;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            return await bookRepository.DeleteBookAsync(id);
        }

        private async Task<bool> BookExistsAsync(string title, List<Author> authorNames)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            if (authorNames.Count == 0)
                throw new ArgumentException("At least one author name is required.", nameof(authorNames));
            
            return await bookRepository.BookExistsAsync(title, authorNames);
        }
    }
}
