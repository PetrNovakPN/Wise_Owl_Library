using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Entities;

namespace Wise_Owl_Library.Extensions
{
    public static class BookEntityExtension
    {
        public static BookEntity ToBookEntity(this Book book) => new()
        {
            Id = book.Id,
            Title = book.Title,
            Price = book.Price,
            Stock = book.Stock,
            Authors = book.Authors.Select(a => new AuthorEntity { Id = a.Id, Name = a.Name }).ToList()
        };
    }
}
