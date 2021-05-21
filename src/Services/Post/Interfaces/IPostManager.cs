using Post.Application.Models;
using Post.Database.Models;
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
        Task<Guid> CreatePostAsync(CreatePostRequest postToCreate);

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
    }
}
