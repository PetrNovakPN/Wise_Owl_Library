using System.ComponentModel.DataAnnotations;

namespace Wise_Owl_Library.Data.Dto.Requests
{
    public class CreateBookDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title length can't be more than 100 characters.")]
        public required string Title { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public double Price { get; set; }

        [Range(0, 10000, ErrorMessage = "Stock must be between 0 and 10000.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Authors are required.")]
        [MinLength(1, ErrorMessage = "At least one author is required.")]
        public required List<AuthorDto> Authors { get; set; }
    }
}
