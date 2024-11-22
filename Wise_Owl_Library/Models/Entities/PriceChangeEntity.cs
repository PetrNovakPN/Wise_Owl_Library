using System.ComponentModel.DataAnnotations.Schema;

namespace Wise_Owl_Library.Models.Entities
{
    public class PriceChangeEntity
    {
        public int Id { get; set; }
        public required Book Book { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal OldPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal NewPrice { get; set; }
        public DateTimeOffset ChangeDate { get; set; }
    }
}
