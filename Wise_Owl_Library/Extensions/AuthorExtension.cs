using Wise_Owl_Library.Models;
using Wise_Owl_Library.Models.Entities;

namespace Wise_Owl_Library.Extensions
{
    public static class AuthorExtension
    {
        public static AuthorEntity ToAuthorEntity(this Author author) => new()
        {
            Id = author.Id,
            Name = author.Name
        };
    }
}
