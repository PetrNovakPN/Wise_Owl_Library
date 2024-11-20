using Wise_Owl_Library.Data.Dto.Requests;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Extensions
{
    public static class CreateBookDtoExtension
    {
        public static CreateBookDto ToCreateBookDto(this Book book) => new()
        {
            Title = book.Title,
            Price = book.Price,
            Stock = book.Stock,
            Authors = book.Authors.Select(a => new AuthorDto { Name = a.Name }).ToList()
        };
    }
}
