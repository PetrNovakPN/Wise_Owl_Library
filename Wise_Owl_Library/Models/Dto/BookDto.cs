using Wise_Owl_Library.Data.Dto.Requests;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Data.Dto
{
    public class BookDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public required List<AuthorDto> Authors { get; set; }
    }
}