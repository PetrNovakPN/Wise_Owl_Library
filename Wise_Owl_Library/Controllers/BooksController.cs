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
    public class BooksController(ApplicationDbContext context, ILogger<BooksController> logger)
        : ControllerBase
    {
        // GET: api/Books
        // Retrieves a list of books, optionally filtered by title and stock.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(string? title = null, int? stock = null)
        {
            try
            {
                // Validate input parameters
                if (title?.Length > 100)
                {
                    return BadRequest("Title length can't be more than 100 characters.");
                }

                if (stock is < 0 or > 10000)
                {
                    return BadRequest("Stock must be between 0 and 10000.");
                }

                // Build query with optional filters
                IQueryable<Book> query = context.Books
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(b => b.Title.Contains(title));
                }

                if (stock.HasValue)
                {
                    query = query.Where(b => b.Stock == stock.Value);
                }

                // Execute query and map results to DTO
                List<Book> books = await query.ToListAsync();
                List<BookDto> bookDto = books.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    Stock = b.Stock,
                    Authors = b.BookAuthors.Select(ba => ba.Author!.Name).ToList()
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
        // Retrieves a specific book by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                // Find book by ID
                Book? book = await context.Books
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return NotFound();
                }

                // Map book to DTO
                BookDto bookDto = new()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Price = book.Price,
                    Stock = book.Stock,
                    Authors = book.BookAuthors.Select(ba => ba.Author!.Name).ToList()
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
        // Creates a new book.
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook([FromBody] CreateBookDto createBookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check for duplicate book
                bool bookExists = await context.Books.AnyAsync(b => b.Title == createBookDto.Title && b.BookAuthors.Any(ba => createBookDto.Authors.Select(a => a.Name).Contains(ba.Author!.Name)));
                if (bookExists)
                {
                    return Conflict(new { message = "The book already exists." });
                }

                // Create new book entity
                Book book = new()
                {
                    Title = createBookDto.Title,
                    Price = createBookDto.Price,
                    Stock = createBookDto.Stock,
                    BookAuthors = []
                };

                // Add authors to the book
                book.BookAuthors = createBookDto.Authors.Select(a => new BookAuthor { Book = book, Author = new Author { Name = a.Name } }).ToList();


                // Save book to database
                context.Books.Add(book);
                await context.SaveChangesAsync();

                // Return created book DTO
                BookDto bookDto = new()
                {
                    Id = book.Id,
                    Title = book.Title,
                    Price = book.Price,
                    Stock = book.Stock,
                    Authors = book.BookAuthors.Select(ba => ba.Author!.Name).ToList()
                };

                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating book.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // PUT: api/Books/5
        // Updates an existing book by its ID.
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
                // Find book by ID
                Book? book = await context.Books
                    .Include(b => b.BookAuthors)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return NotFound();
                }

                 // Check for price change and log it
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

                // Update book entity
                book.Title = updateBookDto.Title;
                book.Price = updateBookDto.Price;
                book.Stock = updateBookDto.Stock;
                book.BookAuthors = updateBookDto.Authors.Select(a => new BookAuthor { Book = book, Author = new Author { Name = a.Name } }).ToList();

                context.Entry(book).State = EntityState.Modified;

                // Save changes to database
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
        // Deletes a book by its ID.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                // Find book by ID
                Book? book = await context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                // Remove book from database
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

        // Checks if a book exists by its ID.
        private bool BookExists(int id)
        {
            return context.Books.Any(e => e.Id == id);
        }
    }
}
