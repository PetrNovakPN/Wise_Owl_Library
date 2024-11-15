using System.ComponentModel.DataAnnotations.Schema;

namespace Wise_Owl_Library.Models
{
    public class PriceChange
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public required Book Book { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OldPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NewPrice { get; set; }

        public DateTime ChangeDate { get; set; }
    }
}
