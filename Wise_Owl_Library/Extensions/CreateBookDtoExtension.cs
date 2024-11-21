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

        public static Book ToBook(this CreateBookDto createBookDto) => new()
        {
            Title = createBookDto.Title,
            Price = createBookDto.Price,
            Stock = createBookDto.Stock,
            Authors = createBookDto.Authors.Select(a => new Author { Name = a.Name }).ToList()
        };
    }
}
