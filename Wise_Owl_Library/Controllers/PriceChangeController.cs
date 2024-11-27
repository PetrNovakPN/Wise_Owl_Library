using Microsoft.AspNetCore.Mvc;
using Wise_Owl_Library.Extensions.PresentationExtensions;
using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Dto;
using Wise_Owl_Library.Services;

namespace Wise_Owl_Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceChangeController(IPriceChangeService priceChangeService) : ControllerBase
    {

        // GET: api/PriceChange
        [HttpGet] 
        public async Task<ActionResult<PriceChangeDto[]>> GetPriceChanges()
        {
            List<PriceChange> priceChanges = await priceChangeService.GetPriceChangesAsync();

            if (priceChanges.Count == 0)
            {
                return NoContent();
            }

            return Ok(priceChanges.Select(p => p.ToPriceChangeDto()).ToArray());
        }
    }
}
