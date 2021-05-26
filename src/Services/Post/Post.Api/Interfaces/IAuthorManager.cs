using Post.Api.Application.Models;
using System;
using System.Threading.Tasks;

namespace Post.Api.Interfaces
{
    public interface IAuthorManager
    {
        Task<Guid> CreateAuthorAsync(AuthorCreateRequest authorToCreate);

        /// <summary>
        /// Read an author with provided Id
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns>Author response model</returns>
        Task<AuthorResponse> GetAuthorByIdAsync(Guid authorId);

        /// <summary>
        /// Update an author information with those provided in the author parameter
        /// </summary>
        /// <param name="author"></param>
        /// <returns>Void</returns>
        Task UpdateAuthorAsync(AuthorCreateRequest author);

        /// <summary>
        /// Delete an author and all related posts
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns>Void</returns>
        Task DeleteAuthorByIdAsync(Guid authorId);
    }
}
