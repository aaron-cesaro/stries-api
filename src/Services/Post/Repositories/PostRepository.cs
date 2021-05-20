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

            Log.Information($"Inserting Post with id {postId}");

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

            Log.Information($"Post {postId} successfully inserted");

            return postId;
        }

        public async Task<PostEntity> ReadPostAsync(Guid postId)
        {
            PostEntity post = null;

            Log.Information($"Reading Post with id {postId}");

            try
            {
                post = await _postContext.Posts
                 .FirstOrDefaultAsync(p => p.Id == postId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postId} not readed");
            }

            Log.Information($"Post {postId} read successfully");

            return post;
        }

        public async Task UpdatePostAsync(PostEntity post)
        {
            Log.Information($"Updating Post with id {post.Id}");
            try
            {
                _postContext.Posts.Update(post);

                await _postContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {post.Id} not updated");
            }

            Log.Information($"Post {post.Id} updated successfully");
        }

        public async Task DeletePostAsync(Guid postId)
        {
            Log.Information($"Deleting Post with id {postId}");

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

            Log.Information($"Post {postId} deleted successfully");
        }
    }
}