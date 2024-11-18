using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Data.Dto
{
    public class PriceChangeDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public required string BookTitle { get; set; }
        public required List<string> Authors { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTimeOffset ChangeDate { get; set; }
    }

    public static class PriceChangeDtoExtensions
    {
        public static PriceChangeDto ToPriceChangeDto(this Book book, decimal oldPrice, decimal newPrice)
        {
            return new PriceChangeDto
            {
                BookId = book.Id,
                BookTitle = book.Title,
                Authors = book.Authors.Select(a => a.Name).ToList(),
                OldPrice = oldPrice,
                NewPrice = newPrice,
                ChangeDate = DateTimeOffset.Now
            };
        }
    }
}