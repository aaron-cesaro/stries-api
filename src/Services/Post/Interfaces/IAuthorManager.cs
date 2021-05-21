using Post.Application.Models;
using System;
using System.Threading.Tasks;

namespace Post.Interfaces
{
    public interface IAuthorManager
    {
        /// <summary>
        /// Create a new author with provided information
        /// </summary>
        /// <param name="authorToCreate"></param>
        /// <returns>Id of the created author</returns>
        Task<Guid> CreateAuthorAsync(AuthorCreateRequest authorToCreate);

        /// <summary>
        /// Read an author with provided Id
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns>Author response model</returns>
        Task<AuthorResponse> ReadAuthorByIdAsync(Guid authorId);

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
        Task<bool> DeleteAuthorAsync(Guid authorId);
    }
}