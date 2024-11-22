using System.ComponentModel.DataAnnotations;

namespace Wise_Owl_Library.Models.Entities
{
    public class AuthorEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required virtual List<BookEntity> Books { get; set; }
    }
}