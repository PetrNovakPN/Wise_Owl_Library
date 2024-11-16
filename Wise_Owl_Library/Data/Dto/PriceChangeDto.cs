namespace Wise_Owl_Library.Data.Dto
{
    public class PriceChangeDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public required string BookTitle { get; set; }
        public required List<string> Authors { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}