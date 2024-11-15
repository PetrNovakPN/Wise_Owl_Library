using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Interfaces;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Services
{
    public class PriceChangeService(ApplicationDbContext context) : IPriceChangeService
    {
        public async Task<List<PriceChangeDto>> GetPriceChangesAsync()
        {
            List<PriceChange> priceChanges = await context.PriceChanges
                .Include(pc => pc.Book)
                .ThenInclude(b => b.Authors)
                .ToListAsync();

            return priceChanges.Select(pc => new PriceChangeDto
            {
                Id = pc.Id,
                BookId = pc.BookId,
                BookTitle = pc.Book.Title,
                Authors = pc.Book.Authors
                    .Select(a => a.Name)
                    .ToList(),
                OldPrice = pc.OldPrice,
                NewPrice = pc.NewPrice,
                ChangeDate = pc.ChangeDate
            }).ToList();
        }
    }
}
