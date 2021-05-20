using Microsoft.EntityFrameworkCore;
using Post.Database.Contextes;
using Post.Database.Entities;
using Post.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Post.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly PostContext _postContext;

        public PostRepository(PostContext postContext)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
        }

        public async Task<Guid> InsertPostAsync(PostEntity post)
        {
            var postId = Guid.NewGuid();

            try
            {
                post.Id = postId;

                _postContext.Posts.Add(post);
                await _postContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postId} not inserted");
            }

            return postId;
        }

        public async Task<PostEntity> ReadPostAsync(Guid postId)
        {
            PostEntity post = null;

            try
            {
                post = await _postContext.Posts
                 .FirstOrDefaultAsync(p => p.Id == postId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postId} not readed");
            }

            return post;
        }

        public async Task UpdatePostAsync(PostEntity post)
        {
            try
            {
                _postContext.Posts.Update(post);

                await _postContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {post.Id} not updated");
            }
        }

        public async Task DeletePostAsync(Guid postId)
        {
            try
            {
                var postToDelete = await _postContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);

                _postContext.Posts.Remove(postToDelete);

                await _postContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postId} not deleted");
            }
        }
    }
}