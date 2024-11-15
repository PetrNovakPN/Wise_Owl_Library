using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Interfaces;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceChangeController(IPriceChangeService priceChangeService, ILogger<PriceChangeController> logger) : ControllerBase
    {

        // GET: api/PriceChange
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceChangeDto>>> GetPriceChanges()
        {
            try
            {
                List<PriceChangeDto> priceChangeDto = await priceChangeService.GetPriceChangesAsync();
                return Ok(priceChangeDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading price changes.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }
    }
}
