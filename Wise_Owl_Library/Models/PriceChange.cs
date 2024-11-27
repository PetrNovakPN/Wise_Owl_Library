using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wise_Owl_Library.Models
{
    public class PriceChange
    {
        public int Id { get; set; }
        public required Book Book { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTimeOffset ChangeDate { get; set; }
    }
}
