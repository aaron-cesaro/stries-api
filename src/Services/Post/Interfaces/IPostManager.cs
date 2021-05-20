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
    }
}
