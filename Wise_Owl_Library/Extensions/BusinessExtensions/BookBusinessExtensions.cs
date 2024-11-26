using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Entities;

namespace Wise_Owl_Library.Extensions.BusinessExtensions;

public static class BookBusinessExtensions
{
    public static BookEntity ToBookEntity(this Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Price = book.Price,
        Stock = book.Stock,
        Authors = book.Authors.Select(a => a.ToAuthorEntity()).ToList()
    };
    
    public static Book ToBook(this BookEntity bookEntity) => new()
    {
        Id = bookEntity.Id,
        Title = bookEntity.Title,
        Price = bookEntity.Price,
        Stock = bookEntity.Stock,
        Authors = bookEntity.Authors.Select(a => a.ToAuthor()).ToList()
    };
}