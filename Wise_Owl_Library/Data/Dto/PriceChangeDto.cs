namespace Wise_Owl_Library.Data.Dto
{
    public class PriceChangeDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public required string BookTitle { get; set; }
        public List<string> Authors { get; set; }
        public double OldPrice { get; set; }
        public double NewPrice { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}