using Post.Api.Application.Models;
using System;
using System.Threading.Tasks;

namespace Post.Api.Interfaces
{
    public interface IPostManager
    {
        /// <summary>
        /// Create a new Post
        /// </summary>
        /// <param name="postToCreate"></param>
        /// <returns>Returns the id of the Post if created, an empty Guid if author is not present</returns>
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
        /// Publish a Post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>True if the post has been published, false otherwise</returns>
        Task PublishPostAsync(Guid postId);

        /// <summary>
        /// Archive a Post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>True if the post has been archived, false otherwise</returns>
        Task ArchivePostAsync(Guid postId);

        /// <summary>
        /// Permanently remove a post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>Void</returns>
        Task DeletePostByIdAsync(Guid postId);

        /// <summary>
        /// Remove all posts created by an author
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns>Void</returns>
        Task DeleteAllPostsByAuthorIdAsync(Guid authorId);
    }
}
