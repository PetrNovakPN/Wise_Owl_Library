using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Data.Dto.Requests;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Extensions
{
    public static class BookDtoExtension
    {
        public static BookDto ToBookDto(this Book book) => new()
        {
            Id = book.Id,
            Title = book.Title,
            Price = book.Price,
            Stock = book.Stock,
            Authors = book.Authors.Select(a => new AuthorDto {Id = a.Id ,Name = a.Name }).ToList()
        };
    }
}
