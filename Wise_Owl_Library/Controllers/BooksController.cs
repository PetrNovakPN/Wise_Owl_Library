using Microsoft.AspNetCore.Mvc;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Data.Dto.Requests;
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
                return NoContent();
            }

            return Ok(books/*books.Select(book => book.ToDto()).ToArray()*/);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            BookDto? book = await bookService.GetBookAsync(id);

            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }

            return Ok(book);
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BookDto[]>> PostBooks([FromBody] List<CreateBookDto> createBookDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //List<Book> books = createBookDtos.Select(dto => dto.ToBook()).ToList();

            List<Book> createdBooks = await bookService.CreateBooksAsync(createBookDtos);

            return Ok(createdBooks/*.Select(book => book.ToDto()).ToList()*/);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<ActionResult> PutBook(int id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Book updatedBook = updateBookDto.ToBook();
            if (await bookService.UpdateBookAsync(updateBookDto) == null)
            {
                return null;//wasnt updated;
            }
            //if ()
            //{
            //    return NotFound(new { message = $"Book with ID {id} not found." });
            //}

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
