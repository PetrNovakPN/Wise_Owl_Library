using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Data.Dto.Requests;

namespace Wise_Owl_Library.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetBooksAsync(string? title, int? stock);
        Task<BookDto?> GetBookAsync(int id);
        Task<IEnumerable<BookDto>> CreateBooksAsync(List<CreateBookDto> createBookDtos);
        Task<bool> UpdateBookAsync(int id, UpdateBookDto updateBookDto);
        Task<bool> DeleteBookAsync(int id);
    }
}
