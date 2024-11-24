using Microsoft.AspNetCore.Mvc;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Data.Dto.Requests;
using Wise_Owl_Library.Extensions;
using Wise_Owl_Library.Models;
using Wise_Owl_Library.Services;

namespace Wise_Owl_Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(IBookService bookService) : ControllerBase
    {
        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<BookDto[]>> GetBooks(string? title = null, int? stock = null)
        {
            List<Book> books = await bookService.GetBooksAsync(title, stock);

            if(books.Count == 0)
            {
                return Ok(new { message = "No books found." });
            }

            return Ok(books.Select(book => book.ToBookDto()).ToList());
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

            return Ok(book.ToBookDto());
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BookDto[]>> PostBooks([FromBody] List<CreateBookDto> createBookDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Book> books = createBookDtos.Select(bDto => bDto.ToBook()).ToList();

            List<Book> createdBooks = await bookService.CreateBooksAsync(books);
            if (createdBooks.Count == 0)
            {
                return NoContent();
            }
            return Ok(createdBooks.Select(book => book.ToBookDto()).ToArray());
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutBook([FromBody] UpdateBookDto updateBookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await bookService.UpdateBookAsync(updateBookDto.ToBook()) == null)
            {
                return NotFound(new { message = $"Book with ID {updateBookDto.Id} wasn't updated correctly." });
            }

            return Ok(new { message = "The book was successfully updated." });
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBook(int id)
        {
            if (!await bookService.DeleteBookAsync(id))
            {
                return NotFound(new { message = $"Book with ID {id} wasn't deleted." });
            }

            return Ok(new { message = $"Book with ID {id} was successfully deleted." });
        }
    }
}
