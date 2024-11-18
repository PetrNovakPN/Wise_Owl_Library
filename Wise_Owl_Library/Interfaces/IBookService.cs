using Wise_Owl_Library.Data.Dto;
using Wise_Owl_Library.Data.Dto.Requests;
using Wise_Owl_Library.Models;

namespace Wise_Owl_Library.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetBooksAsync(string? title = null, int? stock = null);
        Task<Book?> GetBookAsync(int id);
        Task<IEnumerable<Book>> CreateBooksAsync(List<Book> books);
        Task<Book?> UpdateBookAsync(int id, Book updatedBook);
        Task<bool> DeleteBookAsync(int id);
    }
}
