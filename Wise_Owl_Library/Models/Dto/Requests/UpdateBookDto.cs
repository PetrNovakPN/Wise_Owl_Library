using System.ComponentModel.DataAnnotations;

namespace Wise_Owl_Library.Models.Dto.Requests
{
    public class UpdateBookDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title length can't be more than 100 characters.")]
        public required string Title { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Range(0, 10000, ErrorMessage = "Stock must be between 0 and 10000.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Authors are required.")]
        [MinLength(1, ErrorMessage = "At least one author is required.")]
        public required List<AuthorDto> Authors { get; set; }
    }

    public static class UpdateBookDtoExtensions
    {
        public static Book ToBook(this UpdateBookDto updateBookDto)
        {
            return new Book
            {
                Id = updateBookDto.Id,
                Title = updateBookDto.Title,
                Price = updateBookDto.Price,
                Stock = updateBookDto.Stock,
                Authors = updateBookDto.Authors.Select(a => new Author { Name = a.Name }).ToList()
            };
        }
    }

}
