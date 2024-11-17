using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Interfaces;

namespace Wise_Owl_Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceChangeController : ControllerBase
    {
        private readonly IPriceChangeService _priceChangeService;

        public PriceChangeController(IPriceChangeService priceChangeService)
        {
            _priceChangeService = priceChangeService;
        }

        // GET: api/PriceChange
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceChangeDto>>> GetPriceChanges()
        {
            var priceChanges = await _priceChangeService.GetPriceChangesAsync();

            if (priceChanges.Count == 0)
            {
                return NoContent();
            }

            var priceChangeDtos = priceChanges.Select(pc => new PriceChangeDto
            {
                Id = pc.Id,
                BookId = pc.BookId,
                BookTitle = pc.Book.Title,
                Authors = pc.Book.Authors.Select(a => a.Name).ToList(),
                OldPrice = pc.OldPrice,
                NewPrice = pc.NewPrice,
                ChangeDate = pc.ChangeDate
            }).ToList();

            return Ok(priceChangeDtos);
        }
    }
}
