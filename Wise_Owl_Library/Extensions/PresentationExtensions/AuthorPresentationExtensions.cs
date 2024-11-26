using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Dto.Requests;

namespace Wise_Owl_Library.Extensions.PresentationExtensions;

public static class AuthorPresentationExtensions
{
    public static AuthorDto ToAuthorDto(this Author author) => new()
    {
        Id = author.Id,
        Name = author.Name
    };
    
    public static Author ToAuthor(this AuthorDto authorDto) => new()
    {
        Id = authorDto.Id,
        Name = authorDto.Name
    };
}