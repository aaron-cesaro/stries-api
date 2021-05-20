using Post.Database.Entities;
using System;
using System.Threading.Tasks;

namespace Post.Interfaces
{
    public interface IPostRepository
    {
        Task<Guid> InsertPostAsync(PostEntity post);
        Task<PostEntity> ReadPostAsync(Guid postId);
        Task UpdatePostAsync(PostEntity post);
        Task DeletePostAsync(Guid postId);

    }
}