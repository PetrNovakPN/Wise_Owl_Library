using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Data.Dto.Requests;
using Wise_Owl_Library.Extensions;
using Wise_Owl_Library.Models;
using Wise_Owl_Library.Repositories;

namespace Wise_Owl_Library.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksAsync(string? title = null, int? stock = null);
        Task<BookDto?> GetBookAsync(int id);
        Task<List<Book>> CreateBooksAsync(List<Book> books);
        Task<Book?> UpdateBookAsync(int id, Book updatedBook);
        Task<bool> DeleteBookAsync(int id);
    }

    public class BookService(IBookRepository bookRepository, ILogger<BookService> logger) : IBookService
    {
        public async Task<List<Book>> GetBooksAsync(string? title, int? stock)
        {
            return await bookRepository.GetBooksAsync(title, stock);

            //try
            //{
            //    IQueryable<Book> query = context.Books
            //        .Include(b => b.Authors)
            //        .AsQueryable();

            //    if (!string.IsNullOrEmpty(title))
            //    {
            //        query = query.Where(b => b.Title.Contains(title));
            //    }

            //    if (stock.HasValue)
            //    {
            //        query = query.Where(b => b.Stock == stock.Value);
            //    }

            //    return await query.ToListAsync();
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError(ex, "Error loading books.");
            //    throw;
            //}
        }

        public async Task<BookDto?> GetBookAsync(int id)
        {
            Book? book = await bookRepository.GetBookAsync(id);
            if(book == null)
            {
                return null;
            }
            return book.ToBookDto();
            //try
            //{
            //    return await context.Books
            //        .Include(b => b.Authors)
            //        .FirstOrDefaultAsync(b => b.Id == id);
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError(ex, "Error loading book.");
            //    throw;
            //}
        }

        public async Task<List<BookDto>> CreateBooksAsync(List<BookDto> books)
        {
            List<BookDto> createdBooks = [];

            foreach (BookDto book in books)
            {
                if (!await BookExistsAsync(book.Title, book.Authors))
                {
                    context.Books.Add(book);
                    createdBooks.Add(book);
                }
            }

            await context.SaveChangesAsync();
            return createdBooks;
        }

        public async Task<Book?> UpdateBookAsync(int id, Book updatedBook)
        {
            try
            {
                Book? book = await context.Books
                    .Include(b => b.Authors)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return null;
                }

                if (book.Price != updatedBook.Price)
                {
                    AddPriceChange(book, updatedBook.Price);
                }

                book.Title = updatedBook.Title;
                book.Price = updatedBook.Price;
                book.Stock = updatedBook.Stock;
                book.Authors = updatedBook.Authors.Select(a => new Author { Name = a.Name }).ToList();

                context.Entry(book).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return book;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookExistsAsync(id))
                {
                    return null;
                }
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating book with ID {BookId}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                Book? book = await context.Books.FindAsync(id);
                if (book == null)
                {
                    return false;
                }

                context.Books.Remove(book);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating book with ID {BookId}.", id);
                throw;

            }
        }

        private async Task<bool> BookExistsAsync(string title, List<Author> authorNames)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
            if (authorNames.Count == 0)
                throw new ArgumentException("At least one author name is required.", nameof(authorNames));

            return await bookRepository.BookExistsAsync(title, authorNames);
        }

        private async Task<bool> BookExistsAsync(int id)
        {
            return await bookRepository.BookExistsByIdAsync(id);
        }

        private void AddPriceChange(Book book, decimal newPrice)
        {
            PriceChange priceChange = new()
            {
                BookId = book.Id,
                OldPrice = book.Price,
                NewPrice = newPrice,
                ChangeDate = DateTimeOffset.UtcNow
            };
            context.PriceChanges.Add(priceChange);
        }
    }
}
