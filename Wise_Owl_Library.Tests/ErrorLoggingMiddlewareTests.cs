using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Wise_Owl_Library.Middleware;
using Xunit;

namespace Wise_Owl_Library.Tests
{
    public class ErrorLoggingMiddlewareTests
    {
        private readonly Mock<ILogger<ErrorLoggingMiddleware>> _loggerMock;
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly DefaultHttpContext _httpContext;
        private readonly ErrorLoggingMiddleware _middleware;

        public ErrorLoggingMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<ErrorLoggingMiddleware>>();
            _nextMock = new Mock<RequestDelegate>();
            _httpContext = new DefaultHttpContext();
            _httpContext.Response.Body = new MemoryStream();
            _middleware = new ErrorLoggingMiddleware(_nextMock.Object, _loggerMock.Object);
        }


        [Fact]
        public async Task InvokeAsync_Should_Log_Error_And_Set_Response_When_ArgumentException_Occurs()
        {
            // Arrange
            var exception = new ArgumentException("Invalid parameter");
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            _httpContext.Response.Body = new MemoryStream();

            // Act
            await _middleware.InvokeAsync(_httpContext!);

            // Assert: Check if logger was called
            _loggerMock.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An unhandled exception has occurred.")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

            // Assert: Check response status code and content type
            Assert.Equal(StatusCodes.Status400BadRequest, _httpContext.Response.StatusCode);
            Assert.Equal("application/json", _httpContext.Response.ContentType);

            // Assert: Check if the response body contains the error details
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

            Assert.Equal("Invalid parameters", errorResponse.GetProperty("title").GetString());
            Assert.Equal("Invalid parameter", errorResponse.GetProperty("detail").GetString());
        }


        [Fact]
        public async Task InvokeAsync_Should_Return_Correct_Response_For_KeyNotFoundException()
        {
            // Arrange
            var exception = new KeyNotFoundException("Book not found");
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert: Check response status code
            Assert.Equal(StatusCodes.Status404NotFound, _httpContext.Response.StatusCode);

            // Assert: Check if the response body contains the error details
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
            Assert.Equal("Resource not found", errorResponse.GetProperty("title").GetString());
            Assert.Equal("Book not found", errorResponse.GetProperty("detail").GetString());
        }

        [Fact]
        public async Task InvokeAsync_Should_Return_Correct_Response_For_DbUpdateException()
        {
            // Arrange
            var exception = new DbUpdateException("Database update failed");
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert: Check response status code
            Assert.Equal(StatusCodes.Status500InternalServerError, _httpContext.Response.StatusCode);

            // Assert: Check if the response body contains the error details
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
            Assert.Equal("Database error", errorResponse.GetProperty("title").GetString());
            Assert.Equal("Database update failed", errorResponse.GetProperty("detail").GetString());
        }

        [Fact]
        public async Task InvokeAsync_Should_Return_Correct_Response_For_TimeoutException()
        {
            // Arrange
            var exception = new TimeoutException("Request timed out");
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert: Check response status code
            Assert.Equal(StatusCodes.Status504GatewayTimeout, _httpContext.Response.StatusCode);

            // Assert: Check if the response body contains the error details
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
            Assert.Equal("Timeout occurred", errorResponse.GetProperty("title").GetString());
            Assert.Equal("The server took too long to process the request.", errorResponse.GetProperty("detail").GetString());
        }

        [Fact]
        public async Task InvokeAsync_Should_Return_Generic_Error_For_Unhandled_Exception()
        {
            // Arrange
            var exception = new Exception("Unexpected error");
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert: Check response status code
            Assert.Equal(StatusCodes.Status500InternalServerError, _httpContext.Response.StatusCode);

            // Assert: Check if the response body contains the error details
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(_httpContext.Response.Body).ReadToEndAsync();
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
            Assert.Equal("Internal server error", errorResponse.GetProperty("title").GetString());
            Assert.Equal("An unexpected error occurred. Please contact support if the issue persists.", errorResponse.GetProperty("detail").GetString());
        }

        [Fact]
        public async Task InvokeAsync_Should_Call_Next_Middleware_If_No_Exception()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(_httpContext);

            // Assert: Ensure that the next middleware was called once
            _nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        }
    }
}
