using System.ComponentModel.DataAnnotations;

namespace Wise_Owl_Library.Data.Dto.Requests
{
    public class AuthorDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Author name is required.")]
        [StringLength(50, ErrorMessage = "Author name length can't be more than 50 characters.")]
        public required string Name { get; set; }
    }
}
