using Post.Application.Models;
using System;
using System.Threading.Tasks;

namespace Post.Interfaces
{
    public interface IPostManager
    {
        /// <summary>
        /// Create a new Post
        /// </summary>
        /// <param name="postToCreate"></param>
        /// <returns>Id of the created Post</returns>
        Task<Guid> CreatePostAsync(PostCreateRequest postToCreate);

        /// <summary>
        /// Get a Post with provided postId
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>The Post with provided postId</returns>
        Task<PostReponse> GetPostByIdAsync(Guid postId);

        /// <summary>
        /// Update a Post with financial or user generated content
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="newPostData"></param>
        /// <returns>True if the post has been saved, false otherwise</returns>
        Task SavePostAsync(Guid postId, PostData newPostData);

        /// <summary>
        /// Publish a Post with financial or user generated content
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>True if the post has been published, false otherwise</returns>
        Task PublishPostAsync(Guid postId);

        /// <summary>
        /// Permanently remove a post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>Void</returns>
        Task RemovePostAsync(Guid postId);

        /// <summary>
        /// Remove all posts created by an author
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns>Void</returns>
        Task RemoveAllPostsByAuthorAsync(Guid authorId);

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
