using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Interfaces;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceChangeController(IPriceChangeService priceChangeService, IBookService bookService) : ControllerBase
    {

        // GET: api/PriceChange
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceChangeDto>>> GetPriceChanges()
        {
            List<PriceChange> priceChanges = await priceChangeService.GetPriceChangesAsync();

            if (priceChanges.Count == 0)
            {
                return NoContent();
            }

            List<PriceChangeDto> priceChangeDetails = new();

            foreach (PriceChange pc in priceChanges)
            {
                Book? book = await bookService.GetBookAsync(pc.BookId);
                if (book == null)
                {
                    return NotFound(new { message = $"Book with ID {pc.BookId} not found." });
                }

                priceChangeDetails.Add(book.ToPriceChangeDto(pc.OldPrice, pc.NewPrice));
            }

            return Ok(priceChangeDetails);
        }
    }
}
