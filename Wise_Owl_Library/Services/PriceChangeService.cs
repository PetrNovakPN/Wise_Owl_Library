using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Interfaces;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Services
{
    public class PriceChangeService(ApplicationDbContext context) : IPriceChangeService
    {
        public async Task<List<PriceChange>> GetPriceChangesAsync()
        {
            return await context.PriceChanges
                .Include(pc => pc.Book)
                .ThenInclude(b => b.Authors)
                .ToListAsync();
        }
    }
}