using Microsoft.AspNetCore.Mvc;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Data.Dto.Requests;
using Wise_Owl_Library.Interfaces;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(IBookService bookService) : ControllerBase
    {

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(string? title = null, int? stock = null)
        {
            IEnumerable<Book> books = await bookService.GetBooksAsync(title, stock);
            if (!books.Any())
            {
                return Ok(new List<BookDto>());
            }
            return Ok(books.Select(book => new BookDto(book)).ToList());
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            Book book = await bookService.GetBookAsync(id) ?? throw new KeyNotFoundException($"Book with ID {id} not found.");
            return Ok(new BookDto(book));
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<IEnumerable<BookDto>>> PostBooks([FromBody] List<CreateBookDto> createBookDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Book> books = createBookDtos.Select(dto => new Book
            {
                Title = dto.Title,
                Price = dto.Price,
                Stock = dto.Stock,
                Authors = dto.Authors.Select(a => new Author { Name = a.Name }).ToList()
            }).ToList();

            IEnumerable<Book> createdBooks = await bookService.CreateBooksAsync(books);
            return Ok(createdBooks.Select(book => new BookDto(book)).ToList());
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (id != updateBookDto.Id)
            {
                throw new ArgumentException("The provided ID does not match the book ID.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Book updatedBook = new()
            {
                Id = updateBookDto.Id,
                Title = updateBookDto.Title,
                Price = updateBookDto.Price,
                Stock = updateBookDto.Stock,
                Authors = updateBookDto.Authors.Select(a => new Author { Name = a.Name }).ToList()
            };

            bool result = await bookService.UpdateBookAsync(id, updatedBook);
            if (!result)
            {
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }

            return Ok(new { message = "The book was successfully updated." });
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            bool result = await bookService.DeleteBookAsync(id);
            if (!result)
            {
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }

            return NoContent();
        }
    }
}
