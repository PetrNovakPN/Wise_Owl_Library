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
        public async Task<IEnumerable<BookDto>> GetBooksAsync(string? title, int? stock)
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

                List<Book> books = await query.ToListAsync();
                return books.Select(MapToBookDto).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading books.");
                throw;
            }
        }

        public async Task<BookDto?> GetBookAsync(int id)
        {
            try
            {
                Book? book = await context.Books
                    .Include(b => b.Authors)
                    .FirstOrDefaultAsync(b => b.Id == id);

                return book == null ? null : MapToBookDto(book);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading book.");
                throw;
            }
        }

        public async Task<IEnumerable<BookDto>> CreateBooksAsync(List<CreateBookDto> createBookDtos)
        {
            try
            {
                List<BookDto> createdBooks = [];

                foreach (CreateBookDto createBookDto in createBookDtos)
                {
                    if (await BookExistsAsync(createBookDto.Title, createBookDto.Authors.Select(a => a.Name).ToList()))
                    {
                        throw new InvalidOperationException($"The book '{createBookDto.Title}' already exists.");
                    }

                    Book book = new()
                    {
                        Title = createBookDto.Title,
                        Price = createBookDto.Price,
                        Stock = createBookDto.Stock,
                        Authors = createBookDto.Authors.Select(a => new Author { Name = a.Name }).ToList()
                    };

                    context.Books.Add(book);
                    await context.SaveChangesAsync();

                    createdBooks.Add(MapToBookDto(book));
                }

                return createdBooks;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating books.");
                throw;
            }
        }

        public async Task<bool> UpdateBookAsync(int id, UpdateBookDto updateBookDto)
        {
            if (id != updateBookDto.Id)
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

                if (book.Price != updateBookDto.Price)
                {
                    AddPriceChange(book, updateBookDto.Price);
                }

                UpdateBookDetails(book, updateBookDto);

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

        private static BookDto MapToBookDto(Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Price = book.Price,
                Stock = book.Stock,
                Authors = book.Authors.Select(a => a.Name).ToList()
            };
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

        private static void UpdateBookDetails(Book book, UpdateBookDto updateBookDto)
        {
            book.Title = updateBookDto.Title;
            book.Price = updateBookDto.Price;
            book.Stock = updateBookDto.Stock;
            book.Authors = updateBookDto.Authors.Select(a => new Author { Name = a.Name }).ToList();
        }
    }
}
