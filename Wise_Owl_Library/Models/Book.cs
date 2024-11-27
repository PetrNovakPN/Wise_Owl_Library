using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wise_Owl_Library.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public List<Author> Authors { get; set; } = [];
    }
}