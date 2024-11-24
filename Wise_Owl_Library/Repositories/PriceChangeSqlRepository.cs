using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Extensions;
using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Entities;

namespace Wise_Owl_Library.Repositories
{
    public interface IPriceChangeRepository
    {
        /// <summary>
        /// cokoliv
        /// </summary>
        /// <returns></returns>
        Task<List<PriceChange>> GetPriceChangesAsync();
        Task<PriceChange> AddPriceChangeAsync(PriceChange priceChange);
        Task DeletePriceChangeAsync(int id);

    }

    public class PriceChangeSqlRepository(WiseOwlLibraryDbContext wiseOwlLibraryContext) : IPriceChangeRepository
    {
        public async Task<PriceChange> AddPriceChangeAsync(PriceChange priceChange)
        {
            await wiseOwlLibraryContext.PriceChanges.AddAsync(priceChange.ToPriceChangeEntity());
            return priceChange;
        }

        public async Task DeletePriceChangeAsync(int id)
        {
            PriceChangeEntity priceChange = await wiseOwlLibraryContext.PriceChanges.SingleAsync(x => x.Id == id);
            wiseOwlLibraryContext.PriceChanges.Remove(priceChange);
        }

        public async Task<List<PriceChange>> GetPriceChangesAsync()
        {
            return await wiseOwlLibraryContext.PriceChanges.Select(pc => pc.ToPriceChange()).ToListAsync();
        }
    }
}
