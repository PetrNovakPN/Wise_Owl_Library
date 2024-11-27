using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Entities;

namespace Wise_Owl_Library.Extensions.BusinessExtensions;

public static class AuthorBusinessExtensions
{
    public static AuthorEntity ToAuthorEntity(this Author author) => new()
    {
        Id = author.Id,
        Name = author.Name
    };
    
    public static Author ToAuthor(this AuthorEntity authorEntity) => new()
    {
        Id = authorEntity.Id,
        Name = authorEntity.Name
    };
}