using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Models;
using Wise_Owl_Library.Repositories;

namespace Wise_Owl_Library.Services
{
    public interface IPriceChangeService
    {
        Task<List<PriceChange>> GetPriceChangesAsync();
    }

    public class PriceChangeService(IPriceChangeRepository priceChangeRepository) : IPriceChangeService
    {
        public async Task<List<PriceChange>> GetPriceChangesAsync()
        {
            return await priceChangeRepository.GetPriceChangesAsync();
        }
    }
}