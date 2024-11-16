using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Interfaces;

namespace Wise_Owl_Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceChangeController(IPriceChangeService priceChangeService) : ControllerBase
    {

        // GET: api/PriceChange
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceChangeDto>>> GetPriceChanges()
        {

            List<PriceChangeDto> priceChangeDtos;

           
            priceChangeDtos = await priceChangeService.GetPriceChangesAsync();

            if (priceChangeDtos.Count == 0)
            {
                throw new KeyNotFoundException("No price changes found.");
            }

            return Ok(priceChangeDtos);
        }
    }
}