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

namespace Wise_Owl_Library.Tests
{
    public class PriceChangeControllerTests
    {
        private readonly Mock<IPriceChangeService> _mockPriceChangeService;
        private readonly PriceChangeController _controller;

        public PriceChangeControllerTests()
        {
            _mockPriceChangeService = new Mock<IPriceChangeService>();
            _controller = new PriceChangeController(_mockPriceChangeService.Object);
        }

        [Fact]
        public async Task GetPriceChanges_ReturnsOkResult_WithPriceChangeDtos()
        {
            // Arrange
            var priceChangeDtos = new List<PriceChangeDto>
                {
                    new() {
                        Id = 1,
                        BookId = 1,
                        BookTitle = "Book 1",
                        Authors = ["Author 1"],
                        OldPrice = 10.99m,
                        NewPrice = 12.99m,
                        ChangeDate = DateTime.UtcNow
                    }
                };
            _mockPriceChangeService.Setup(service => service.GetPriceChangesAsync()).ReturnsAsync(priceChangeDtos);

            // Act
            var result = await _controller.GetPriceChanges();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<PriceChangeDto>>(okResult.Value);
            Assert.Equal(priceChangeDtos.Count, returnValue.Count);
        }

        [Fact]
        public async Task GetPriceChanges_ThrowsKeyNotFoundException_WhenNoPriceChangesFound()
        {
            // Arrange
            _mockPriceChangeService.Setup(service => service.GetPriceChangesAsync()).ReturnsAsync([]);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetPriceChanges());
        }
    }
}
