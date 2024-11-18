using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Data.Dto
{
    public class BookDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public required List<string> Authors { get; set; }
    }

    public static class BookExtensions
    {
        public static BookDto ToDto(this Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Price = book.Price,
                Stock = book.Stock,
                Authors = book.Authors.Select(a => a.Name).ToList()
            };
        }
    }
}