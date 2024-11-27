using System.ComponentModel.DataAnnotations;
using Wise_Owl_Library.Validation;

namespace Wise_Owl_Library.Models.Dto.Requests
{
    public class AuthorDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Author name is required.")]
        [StringLength(50, ErrorMessage = "Author name length can't be more than 50 characters.")]
        [NoEmptyOrExcessiveSpaces(ErrorMessage = "Name cannot contain multiple consecutive spaces or start/end with spaces.")]
        public required string Name { get; set; }
    }
}
