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
            try
            {
                foreach (Book book in books)
                {
                    if (await BookExistsAsync(book.Title, book.Authors.Select(a => a.Name).ToList()))
                    {
                        throw new InvalidOperationException($"The book '{book.Title}' already exists.");
                    }

                    context.Books.Add(book);
                }

                await context.SaveChangesAsync();
                return books;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating books.");
                throw;
            }
        }

        public async Task<bool> UpdateBookAsync(int id, Book updatedBook)
        {
            if (id != updatedBook.Id)
            {
                throw new ArgumentException("ID mismatch.");
            }

            try
            {
                Book? book = await context.Books
                    .Include(b => b.Authors)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return false;
                }

                if (book.Price != updatedBook.Price)
                {
                    AddPriceChange(book, updatedBook.Price);
                }

                UpdateBookDetails(book, updatedBook);

                context.Entry(book).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating book.");
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
                logger.LogError(ex, "Error deleting book.");
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
                Book = book,
                OldPrice = book.Price,
                NewPrice = newPrice,
                ChangeDate = DateTime.UtcNow
            };
            context.PriceChanges.Add(priceChange);
        }

        private static void UpdateBookDetails(Book book, Book updatedBook)
        {
            book.Title = updatedBook.Title;
            book.Price = updatedBook.Price;
            book.Stock = updatedBook.Stock;
            book.Authors = updatedBook.Authors.Select(a => new Author { Name = a.Name }).ToList();
        }
    }
}
