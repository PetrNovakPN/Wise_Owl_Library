using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Entities;

namespace Wise_Owl_Library.Extensions
{
    public static class BookExtension
    {
        public static Book ToBook(this BookDto bookDto) => new()
        {
            Id = bookDto.Id,
            Title = bookDto.Title,
            Price = bookDto.Price,
            Stock = bookDto.Stock,
            Authors = bookDto.Authors.Select(a => new Author { Id = a.Id, Name = a.Name }).ToList()
        };

        public static Book EntityToBook(this BookEntity bookEntity) => new()
        {
            Id = bookEntity.Id,
            Title = bookEntity.Title,
            Price = bookEntity.Price,
            Stock = bookEntity.Stock,
            Authors = bookEntity.Authors.Select(a => new Author { Id = a.Id, Name = a.Name }).ToList()
        };
    }
}
