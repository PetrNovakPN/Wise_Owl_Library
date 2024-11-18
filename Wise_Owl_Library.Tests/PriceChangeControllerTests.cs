using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Wise_Owl_Library.Controllers;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Interfaces;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Tests
{
    public class PriceChangeControllerTests
    {
        private readonly Mock<IPriceChangeService> _mockPriceChangeService;
        private readonly Mock<IBookService> _mockBookService;
        private readonly PriceChangeController _controller;

        public PriceChangeControllerTests()
        {
            _mockPriceChangeService = new Mock<IPriceChangeService>();
            _mockBookService = new Mock<IBookService>();
            _controller = new PriceChangeController(_mockPriceChangeService.Object, _mockBookService.Object);
        }

        [Fact]
        public async Task GetPriceChanges_ReturnsOkResult_WithPriceChangeDetails()
        {
            // Arrange
            var priceChanges = new List<PriceChange>
                    {
                        new() {
                            Id = 1,
                            BookId = 1,
                            OldPrice = 10.99m,
                            NewPrice = 12.99m,
                            ChangeDate = DateTimeOffset.UtcNow
                        }
                    };

            var book = new Book
            {
                Id = 1,
                Title = "Book 1",
                Authors = [new Author { Name = "Author 1" }]
            };

            _mockPriceChangeService.Setup(service => service.GetPriceChangesAsync()).ReturnsAsync(priceChanges);
            _mockBookService.Setup(service => service.GetBookAsync(1)).ReturnsAsync(book);

            // Act
            var result = await _controller.GetPriceChanges();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<PriceChangeDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal("Book 1", returnValue[0].BookTitle);
        }

        [Fact]
        public async Task GetPriceChanges_ReturnsNoContent_WhenNoPriceChangesFound()
        {
            // Arrange
            _mockPriceChangeService.Setup(service => service.GetPriceChangesAsync()).ReturnsAsync([]);

            // Act
            var result = await _controller.GetPriceChanges();

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetPriceChanges_ReturnsNotFound_WhenBookNotFound()
        {
            // Arrange
            var priceChanges = new List<PriceChange>
                    {
                        new() {
                            Id = 1,
                            BookId = 1,
                            OldPrice = 10.99m,
                            NewPrice = 12.99m,
                            ChangeDate = DateTimeOffset.UtcNow
                        }
                    };

            _mockPriceChangeService.Setup(service => service.GetPriceChangesAsync()).ReturnsAsync(priceChanges);
            _mockBookService.Setup(service => service.GetBookAsync(1)).ReturnsAsync((Book?)null);

            // Act
            var result = await _controller.GetPriceChanges();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.NotNull(notFoundResult.Value);
            var message = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("Book with ID 1 not found.", message);
        }
    }
}
