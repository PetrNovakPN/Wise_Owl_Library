using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Interfaces
{
    public interface IPriceChangeService
    {
        Task<List<PriceChange>> GetPriceChangesAsync();
    }
}
