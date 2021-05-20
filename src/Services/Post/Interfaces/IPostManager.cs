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
        /// Update and publish (if status is "publish") a Post with financial or user generated content
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="newPostData"></param>
        /// <returns>True if the post has been saved and/or published, false otherwise</returns>
        Task<bool> SaveAndPusblishPostAsync(Guid postId, string status, PostData newPostData);

        /// <summary>
        /// Permanently remove a post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>Void</returns>
        Task RemovePostAsync(Guid postId);
    }
}
