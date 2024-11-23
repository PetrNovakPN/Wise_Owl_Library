using System.ComponentModel.DataAnnotations.Schema;

namespace Wise_Owl_Library.Models.Entities
{
    public class BookEntity
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public required List<AuthorEntity> Authors { get; set; }
    }
}
