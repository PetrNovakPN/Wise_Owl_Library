using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Models;

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
            await wiseOwlLibraryContext.PriceChanges.AddAsync(priceChange);
            return priceChange;
        }

        public async Task DeletePriceChangeAsync(int id)
        {
            PriceChange priceChange = await wiseOwlLibraryContext.PriceChanges.SingleAsync(x => x.Id == id);
            wiseOwlLibraryContext.PriceChanges.Remove(priceChange);
        }

        public async Task<List<PriceChange>> GetPriceChangesAsync()
        {
            return await wiseOwlLibraryContext.PriceChanges.ToListAsync();
        }
    }
}
