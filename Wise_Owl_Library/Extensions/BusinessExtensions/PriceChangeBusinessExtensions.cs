using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Entities;

namespace Wise_Owl_Library.Extensions.BusinessExtensions;

public static class PriceChangeBusinessExtensions
{
    public static PriceChangeEntity ToPriceChangeEntity(this PriceChange priceChange) => new()
    {
        Id = priceChange.Id,
        Book = priceChange.Book.ToBookEntity(),
        OldPrice = priceChange.OldPrice,
        NewPrice = priceChange.NewPrice,
        ChangeDate = priceChange.ChangeDate
    };

    public static PriceChange ToPriceChange(this PriceChangeEntity priceChangeEntity) => new()
    {
        Id = priceChangeEntity.Id,
        Book = priceChangeEntity.Book.ToBook(),
        OldPrice = priceChangeEntity.OldPrice,
        NewPrice = priceChangeEntity.NewPrice,
        ChangeDate = priceChangeEntity.ChangeDate
    };
}