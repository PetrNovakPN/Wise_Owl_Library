using System.ComponentModel.DataAnnotations.Schema;

namespace Wise_Owl_Library.Models
{
    public class PriceChange
    {
        public int Id { get; set; }
        public int BookId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OldPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NewPrice { get; set; }

        public DateTimeOffset ChangeDate { get; set; }
    }
}
