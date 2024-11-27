using Wise_Owl_Library.Models.Dto.Requests;

namespace Wise_Owl_Library.Models.Dto
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