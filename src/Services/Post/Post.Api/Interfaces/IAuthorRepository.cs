using Post.Api.Database.Entities;
using System;
using System.Threading.Tasks;

namespace Post.Api.Interfaces
{
    public interface IAuthorRepository
    {
        /// <summary>
        /// Insert into the database a new Author
        /// </summary>
        /// <param name="author"></param>
        /// <returns>Id of the inserted Author</returns>
        Task InsertAuthorAsync(AuthorEntity author);

        /// <summary>
        /// Read the Author with provided authorId
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns>Author Entity with Id equal to authorId</returns>
        Task<AuthorEntity> ReadAuthorAsync(Guid authorId);

        /// <summary>
        /// Update an already present Author info
        /// </summary>
        /// <param name="author"></param>
        /// <returns>Void</returns>
        Task UpdateAuthorAsync(AuthorEntity author);

        /// <summary>
        /// Delete the Author with the provided authorId
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns>Void</returns>
        Task DeleteAuthorAsync(Guid authorId);
    }
}