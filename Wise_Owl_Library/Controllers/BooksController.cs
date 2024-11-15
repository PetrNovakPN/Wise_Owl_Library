using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Wise_Owl_Library.Data;
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
            try
            {
                IEnumerable<Book> books = await bookService.GetBooksAsync(title, stock);
                List<BookDto> bookDtos = books.Select(book => new BookDto(book)).ToList();
                return Ok(bookDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                Book? book = await bookService.GetBookAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                BookDto bookDto = new(book);
                return Ok(bookDto);
            }
            catch (Exception)
            {
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
                List<Book> books = createBookDtos.Select(dto => new Book
                {
                    Title = dto.Title,
                    Price = dto.Price,
                    Stock = dto.Stock,
                    Authors = dto.Authors.Select(a => new Author { Name = a.Name }).ToList()
                }).ToList();

                IEnumerable<Book> createdBooks = await bookService.CreateBooksAsync(books);
                List<BookDto> createdBookDtos = createdBooks.Select(book => new BookDto(book)).ToList();
                return Ok(createdBookDtos);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
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
                    return NotFound();
                }
                return Ok(new { message = "The book was successfully updated." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                bool result = await bookService.DeleteBookAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }
    }
}
