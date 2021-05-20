using Post.Database.Entities;
using System;
using System.Threading.Tasks;

namespace Post.Interfaces
{
    public interface IPostRepository
    {
        /// <summary>
        /// Insert into the database a new Post
        /// </summary>
        /// <param name="post"></param>
        /// <returns>Id of the inserted Post</returns>
        Task<Guid> InsertPostAsync(PostEntity post);

        /// <summary>
        /// Read the post with provided postId
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>Post Entity with Id equal to postId</returns>
        Task<PostEntity> ReadPostAsync(Guid postId);

        /// <summary>
        /// Update a post content
        /// </summary>
        /// <param name="post"></param>
        /// <returns>Void</returns>
        Task UpdatePostAsync(PostEntity post);

        /// <summary>
        /// Delete the post with provided postId
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>Post author id</returns>
        Task<Guid> DeletePostAsync(Guid postId);

    }
}