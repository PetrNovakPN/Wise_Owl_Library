using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data;
using Wise_Owl_Library.Interfaces;
using Wise_Owl_Library.Middleware;
using Wise_Owl_Library.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddLogging();

builder.Services.AddScoped<IPriceChangeService, PriceChangeService>();
builder.Services.AddScoped<IBookService, BookService>();




WebApplication app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Register middleware
app.UseMiddleware<ErrorLoggingMiddleware>();

app.MapControllers();

app.Run();
