using System.ComponentModel.DataAnnotations;

namespace Wise_Owl_Library.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name length can't be more than 50 characters.")]
        public required string Name { get; set; }

        [Required]
        public List<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
