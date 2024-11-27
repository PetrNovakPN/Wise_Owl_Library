using System.ComponentModel.DataAnnotations;

namespace Wise_Owl_Library.Models
{
    public class Author
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
