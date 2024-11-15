using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Wise_Owl_Library.Middleware
{
    public class ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception has occurred.");

                context.Response.ContentType = "application/json";

                var (statusCode, title, detail) = ex switch
                {
                    ArgumentException argEx => (StatusCodes.Status400BadRequest, "Invalid parameters", argEx.Message),
                    KeyNotFoundException keyEx => (StatusCodes.Status404NotFound, "Resource not found", keyEx.Message),
                    DbUpdateException dbEx => (StatusCodes.Status500InternalServerError, "Database error", dbEx.Message),
                    TimeoutException => (StatusCodes.Status504GatewayTimeout, "Timeout occurred", "The server took too long to process the request."),
                    _ => (StatusCodes.Status500InternalServerError, "Internal server error", "An unexpected error occurred. Please contact support if the issue persists.")
                };

                context.Response.StatusCode = statusCode;

                var errorResponse = new
                {
                    title,
                    detail,
                    statusCode,
                    instance = context.Request.Path.Value,
                    traceId = context.TraceIdentifier

                };

                var jsonResponse = JsonSerializer.Serialize(errorResponse);

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}