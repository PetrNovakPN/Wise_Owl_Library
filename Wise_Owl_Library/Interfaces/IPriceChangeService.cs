using Wise_Owl_Library.Data.Dto;

namespace Wise_Owl_Library.Interfaces
{
    public interface IPriceChangeService
    {
        Task<List<PriceChangeDto>> GetPriceChangesAsync();
    }
}
