namespace Wise_Owl_Library.Models
{
    public class PriceChange
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public required Book Book { get; set; }
        public double OldPrice { get; set; }
        public double NewPrice { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}
