using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wise_Owl_Library.Controllers;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Data.Dto.Requests;
using Wise_Owl_Library.Interfaces;
using Wise_Owl_Library.Models;
using Xunit;

namespace Wise_Owl_Library.Tests
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockBookService = new Mock<IBookService>();
            _controller = new BooksController(_mockBookService.Object);
        }

        [Fact]
        public async Task GetBooks_ReturnsOkResult_WithListOfBooks()
        {
            // Arrange
            var books = new List<Book>
                {
                    new() { Id = 1, Title = "Book 1", Price = 10.99m, Stock = 5, Authors = [new Author { Name = "Author 1" }] },
                    new() { Id = 2, Title = "Book 2", Price = 15.99m, Stock = 3, Authors = [new Author { Name = "Author 2" }] }
                };
            _mockBookService.Setup(service => service.GetBooksAsync(null, null)).ReturnsAsync(books);

            // Act
            var result = await _controller.GetBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBooks = Assert.IsType<List<BookDto>>(okResult.Value);
            Assert.Equal(2, returnBooks.Count);
        }

        [Fact]
        public async Task GetBook_ReturnsOkResult_WithBook()
        {
            // Arrange
            var book = new Book { Id = 1, Title = "Book 1", Price = 10.99m, Stock = 5, Authors = new List<Author> { new Author { Name = "Author 1" } } };
            _mockBookService.Setup(service => service.GetBookAsync(1)).ReturnsAsync(book);

            // Act
            var result = await _controller.GetBook(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBook = Assert.IsType<BookDto>(okResult.Value);
            Assert.Equal(1, returnBook.Id);
        }

        [Fact]
        public async Task PostBooks_ReturnsOkResult_WithCreatedBooks()
        {
            // Arrange
            var createBookDtos = new List<CreateBookDto>
                {
                    new() { Title = "Book 1", Price = 10.99m, Stock = 5, Authors = [new AuthorDto { Name = "Author 1" }] },
                    new() { Title = "Book 2", Price = 15.99m, Stock = 3, Authors = [new AuthorDto { Name = "Author 2" }] }
                };
            var books = createBookDtos.Select(dto => new Book
            {
                Title = dto.Title,
                Price = dto.Price,
                Stock = dto.Stock,
                Authors = dto.Authors.Select(a => new Author { Name = a.Name }).ToList()
            }).ToList();
            _mockBookService.Setup(service => service.CreateBooksAsync(It.IsAny<List<Book>>())).ReturnsAsync(books);

            // Act
            var result = await _controller.PostBooks(createBookDtos);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBooks = Assert.IsType<List<BookDto>>(okResult.Value);
            Assert.Equal(2, returnBooks.Count);
        }

        [Fact]
        public async Task PutBook_ReturnsOkResult_WhenBookIsUpdated()
        {
            // Arrange
            var updateBookDto = new UpdateBookDto { Id = 1, Title = "Updated Book", Price = 12.99m, Stock = 7, Authors = new List<AuthorDto> { new() { Name = "Updated Author" } } };
            _mockBookService.Setup(service => service.UpdateBookAsync(1, It.IsAny<Book>())).ReturnsAsync(true);

            // Act
            var result = await _controller.PutBook(1, updateBookDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnMessage = okResult.Value?.GetType().GetProperty("message")?.GetValue(okResult.Value, null);
            Assert.Equal("The book was successfully updated.", returnMessage);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContentResult_WhenBookIsDeleted()
        {
            // Arrange
            _mockBookService.Setup(service => service.DeleteBookAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBook(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Exception tests
        [Fact]
        public async Task GetBook_ThrowsKeyNotFoundException_WhenBookNotFound()
        {
            // Arrange
            _mockBookService.Setup(service => service.GetBookAsync(It.IsAny<int>())).ReturnsAsync((Book?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetBook(1));
        }

        [Fact]
        public async Task PostBooks_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await _controller.PostBooks([]);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task PutBook_ThrowsArgumentException_WhenIdDoesNotMatch()
        {
            // Arrange
            var updateBookDto = new UpdateBookDto { Id = 2, Title = "Updated Book", Price = 12.99m, Stock = 7, Authors = [new() { Name = "Updated Author" }] };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _controller.PutBook(1, updateBookDto));
        }

        [Fact]
        public async Task PutBook_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Required");
            var updateBookDto = new UpdateBookDto { Id = 1, Title = "Updated Book", Price = 12.99m, Stock = 7, Authors = [new() { Name = "Updated Author" }] };

            // Act
            var result = await _controller.PutBook(1, updateBookDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task PutBook_ThrowsKeyNotFoundException_WhenBookNotFound()
        {
            // Arrange
            var updateBookDto = new UpdateBookDto { Id = 1, Title = "Updated Book", Price = 12.99m, Stock = 7, Authors = new List<AuthorDto> { new() { Name = "Updated Author" } } };
            _mockBookService.Setup(service => service.UpdateBookAsync(It.IsAny<int>(), It.IsAny<Book>())).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.PutBook(1, updateBookDto));
        }

        [Fact]
        public async Task DeleteBook_ThrowsKeyNotFoundException_WhenBookNotFound()
        {
            // Arrange
            _mockBookService.Setup(service => service.DeleteBookAsync(It.IsAny<int>())).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.DeleteBook(1));
        }
    }
}
