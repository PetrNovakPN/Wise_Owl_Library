using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Dto;

namespace Wise_Owl_Library.Extensions.PresentationExtensions;

public static class PriceChangePresentationExtensions
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