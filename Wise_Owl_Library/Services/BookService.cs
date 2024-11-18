using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Data.Dto.Requests;
using Wise_Owl_Library.Interfaces;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Services
{
    public class BookService(ApplicationDbContext context, ILogger<BookService> logger) : IBookService
    {
        public async Task<IEnumerable<Book>> GetBooksAsync(string? title, int? stock)
        {
            try
            {
                IQueryable<Book> query = context.Books
                    .Include(b => b.Authors)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(b => b.Title.Contains(title));
                }

                if (stock.HasValue)
                {
                    query = query.Where(b => b.Stock == stock.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading books.");
                throw;
            }
        }

        public async Task<Book?> GetBookAsync(int id)
        {
            try
            {
                return await context.Books
                    .Include(b => b.Authors)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading book.");
                throw;
            }
        }

        public async Task<IEnumerable<Book>> CreateBooksAsync(List<Book> books)
        {
            List<Book> createdBooks = [];

            foreach (Book book in books)
            {
                if (!await BookExistsAsync(book.Title, book.Authors.Select(a => a.Name).ToList()))
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

        private async Task<bool> BookExistsAsync(string title, List<string> authorNames)
        {
            return await context.Books.AnyAsync(b => b.Title == title && b.Authors.Any(a => authorNames.Contains(a.Name)));
        }

        private async Task<bool> BookExistsAsync(int id)
        {
            return await context.Books.AnyAsync(e => e.Id == id);
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
