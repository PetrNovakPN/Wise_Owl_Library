using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Interfaces;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Services
{
    public class PriceChangeService(ApplicationDbContext context) : IPriceChangeService
    {
        //To je možná až moc jednoduchý na to aby to mělo samostatnou service, ale může se tu něco rozšířit a pak se to nemusí hledat
        public async Task<List<PriceChange>> GetPriceChangesAsync()
        {
            return await context.PriceChanges.ToListAsync();
        }
    }
}