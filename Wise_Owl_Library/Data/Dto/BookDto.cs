using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Data.Dto
{
    public class BookDto(Book book)
    {
        public int Id { get; set; } = book.Id;
        public string Title { get; set; } = book.Title;
        public decimal Price { get; set; } = book.Price;
        public int Stock { get; set; } = book.Stock;
        public List<string> Authors { get; set; } = book.Authors.Select(a => a.Name).ToList();
    }
}