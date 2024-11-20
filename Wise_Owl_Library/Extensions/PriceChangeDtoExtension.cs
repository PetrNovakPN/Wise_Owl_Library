using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Extensions
{
    public static class PriceChangeDtoExtensions
    {
        public static PriceChangeDto ToPriceChangeDto(this PriceChange priceChange ) => new()
        {
            BookId = priceChange.Id,
            BookTitle = priceChange.Book.Title,
            Authors = priceChange.Book.Authors.Select(a => a.Name).ToList(),
            OldPrice = priceChange.OldPrice,
            NewPrice = priceChange.NewPrice,
            ChangeDate = DateTimeOffset.Now
        };
        
    }
}
