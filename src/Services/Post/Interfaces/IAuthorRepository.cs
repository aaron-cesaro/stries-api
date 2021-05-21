using Post.Database.Entities;
using System;
using System.Threading.Tasks;

namespace Post.Interfaces
{
    public interface IAuthorRepository
    {
        Task InsertAuthorAsync(AuthorEntity author);
        Task<AuthorEntity> ReadAuthorAsync(Guid authorId);
        Task UpdateAuthorAsync(AuthorEntity author);
        Task DeleteAuthorAsync(Guid authorId);
    }
}
