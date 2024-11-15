using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Data.Dto.Requests;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(ApplicationDbContext context, ILogger<BooksController> logger) : ControllerBase
    {

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(string? title = null, int? stock = null)
        {
            try
            {
                if (title?.Length > 100)
                {
                    return BadRequest("Title length can't be more than 100 characters.");
                }

                if (stock is < 0 or > 10000)
                {
                    return BadRequest("Stock must be between 0 and 10000.");
                }

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
                List<BookDto> bookDto = books.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    Stock = b.Stock,
                    Authors = b.Authors.Select(a => a.Name).ToList()
                }).ToList();

                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading books.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                Book? book = await context.Books
                    .Include(b => b.Authors)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return NotFound();
                }

                BookDto bookDto = new()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Price = book.Price,
                    Stock = book.Stock,
                    Authors = book.Authors.Select(a => a.Name).ToList()
                };

                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading book.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<IEnumerable<BookDto>>> PostBooks([FromBody] List<CreateBookDto> createBookDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                List<BookDto> createdBooks = [];

                foreach (CreateBookDto createBookDto in createBookDtos)
                {
                    bool bookExists = await context.Books.AnyAsync(b => b.Title == createBookDto.Title && b.Authors.Any(a => createBookDto.Authors.Select(ad => ad.Name).Contains(a.Name)));
                    if (bookExists)
                    {
                        return Conflict(new { message = $"The book '{createBookDto.Title}' already exists." });
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

                    BookDto bookDto = new()
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Price = book.Price,
                        Stock = book.Stock,
                        Authors = book.Authors.Select(a => a.Name).ToList()
                    };

                    createdBooks.Add(bookDto);
                }

                return Ok(createdBooks);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating books.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (id != updateBookDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Book? book = await context.Books
                    .Include(b => b.Authors)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return NotFound();
                }

                if (book.Price != updateBookDto.Price)
                {
                    PriceChange priceChange = new()
                    {
                        BookId = book.Id,
                        Book = book,
                        OldPrice = book.Price,
                        NewPrice = updateBookDto.Price,
                        ChangeDate = DateTime.UtcNow
                    };
                    context.PriceChanges.Add(priceChange);
                }

                book.Title = updateBookDto.Title;
                book.Price = updateBookDto.Price;
                book.Stock = updateBookDto.Stock;
                book.Authors = updateBookDto.Authors.Select(a => new Author { Name = a.Name }).ToList();

                context.Entry(book).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(new { message = "The book was successfully updated." });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error updating book.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating book.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                Book? book = await context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                context.Books.Remove(book);
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting book.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        private bool BookExists(int id)
        {
            return context.Books.Any(e => e.Id == id);
        }
        
    }
}
