using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Dto;
using Wise_Owl_Library.Models.Dto.Requests;

namespace Wise_Owl_Library.Extensions.PresentationExtensions;

public static class BookPresentationExtensions
{
    public static BookDto ToBookDto(this Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Price = book.Price,
        Stock = book.Stock,
        Authors = book.Authors.Select(a => a.ToAuthorDto()).ToList()
    };

    public static Book ToBook(this BookDto book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Price = book.Price,
        Stock = book.Stock,
        Authors = book.Authors.Select(a => a.ToAuthor()).ToList()
    };
    public static Book ToBook(this CreateBookDto createBookDto) => new()
    {
        Title = createBookDto.Title,
        Price = createBookDto.Price,
        Stock = createBookDto.Stock,
        Authors = createBookDto.Authors.Select(a => a.ToAuthor()).ToList()
    };
    
    public static Book ToBook(this UpdateBookDto updateBookDto) => new()
    {
        Id = updateBookDto.Id,
        Title = updateBookDto.Title,
        Price = updateBookDto.Price,
        Stock = updateBookDto.Stock,
        Authors = updateBookDto.Authors.Select(a => new Author { Name = a.Name }).ToList()
    };
    
}