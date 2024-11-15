using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Data;
using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceChangeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PriceChangeController> _logger;

        public PriceChangeController(ApplicationDbContext context, ILogger<PriceChangeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/PriceChange
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceChangeDto>>> GetPriceChanges()
        {
            try
            {
                // Load all price changes from the database including books and authors
                List<PriceChange> priceChanges = await _context.PriceChanges
                    .Include(pc => pc.Book)
                    .ThenInclude(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .ToListAsync();

                // Map price changes to DTO
                List<PriceChangeDto> priceChangeDto = priceChanges.Select(pc => new PriceChangeDto
                {
                    Id = pc.Id,
                    BookId = pc.BookId,
                    BookTitle = pc.Book.Title,
                    Authors = pc.Book.BookAuthors.Select(ba => ba.Author.Name).ToList(),
                    OldPrice = pc.OldPrice,
                    NewPrice = pc.NewPrice,
                    ChangeDate = pc.ChangeDate
                }).ToList();

                return Ok(priceChangeDto);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error loading price changes.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }
    }
}
