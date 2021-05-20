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
        Task<Guid> CreatePostAsync(CreatePostRequest postToCreate);

        /// <summary>
        /// Update a non published Post with updated financial or user generated content
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="newPostData"></param>
        /// <returns>Void</returns>
        Task<bool> UpdatePostAsync(Guid postId, PostData newPostData);

        /// <summary>
        /// Permanently remove a post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>Void</returns>
        Task RemovePostAsync(Guid postId);
    }
}
