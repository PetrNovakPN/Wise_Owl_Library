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

            return Ok(!books.Any() ? new List<BookDto>() : books.Select(book => book.ToDto()).ToList());
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            Book? book = await bookService.GetBookAsync(id);

            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            return Ok(book.ToDto());
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<IEnumerable<BookDto>>> PostBooks([FromBody] List<CreateBookDto> createBookDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Book> books = createBookDetails.Select(dto => dto.ToBook()).ToList();

            IEnumerable<Book> createdBooks = await bookService.CreateBooksAsync(books);

            return Ok(createdBooks.Select(book => book.ToDto()).ToList());
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutBook(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Book updatedBook = updateBookDto.ToBook();

            if (!await bookService.UpdateBookAsync(id, updatedBook))
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            return Ok(new { message = "The book was successfully updated." });
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            if (!await bookService.DeleteBookAsync(id))
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            return NoContent();
        }
    }
}
