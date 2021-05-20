using Post.Application.Models;
using System;
using System.Threading.Tasks;

namespace Post.Interfaces
{
    public interface IPostManager
    {
        Task<Guid> CreatePostAsync(CreatePostRequest postToCreate);
    }
}
