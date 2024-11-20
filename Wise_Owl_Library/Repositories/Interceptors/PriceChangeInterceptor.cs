using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Repositories.Interceptors
{
    public class PriceChangeInterceptor : SaveChangesInterceptor
    {
        //public override int SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        //{
        //    var context = eventData.Context;
        //    if (context != null)
        //    {
        //        DetectPriceChanges(context);
        //    }

        //    return base.SavingChanges(eventData, result);
        //}

        //private void DetectPriceChanges(DbContext context)
        //{
        //    var priceChangeEntries = context.ChangeTracker.Entries<Product>()
        //        .Where(e => e.State == EntityState.Modified &&
        //                    e.Property(p => p.Price).IsModified);

        //    foreach (var entry in priceChangeEntries)
        //    {
        //        var originalPrice = entry.OriginalValues.GetValue<decimal>("Price");
        //        var currentPrice = entry.CurrentValues.GetValue<decimal>("Price");

        //        if (originalPrice != currentPrice)
        //        {
        //            context.Add(new PriceChange
        //            {
        //                ProductId = entry.Entity.Id,
        //                OldPrice = originalPrice,
        //                NewPrice = currentPrice,
        //                ChangeDate = DateTime.UtcNow
        //            });
        //        }
        //    }
        //}
    }
}
