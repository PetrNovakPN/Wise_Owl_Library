using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wise_Owl_Library.Data.Dto.Requests;

namespace Wise_Owl_Library.Models
{
    public class Book
    {
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "Title length can't be more than 100 characters.")]
        public required string Title { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Range(0, 10000, ErrorMessage = "Stock must be between 0 and 10000.")]
        public int Stock { get; set; }

        [MinLength(1, ErrorMessage = "At least one author is required.")]
        public required List<Author> Authors { get; set; }
    }
}