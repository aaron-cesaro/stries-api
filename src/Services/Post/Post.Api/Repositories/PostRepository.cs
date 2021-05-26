using Microsoft.EntityFrameworkCore;
using Post.Api.Database.Contextes;
using Post.Api.Database.Entities;
using Post.Api.Infrastructure.Exceptions;
using Post.Api.Interfaces;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Post.Api.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly PostContext _dbContext;

        public PostRepository(PostContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Guid> InsertPostAsync(PostEntity post)
        {
            try
            {
                post.PostId = Guid.NewGuid();

                _dbContext.Posts.Add(post);
                await _dbContext.SaveChangesAsync();

                return post.PostId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {post.Title} not inserted");
                throw new PostDbOperationNotExecutedException(ex, $"Post title {post.Title}");
            }
        }

        public async Task<PostEntity> ReadPostByIdAsync(Guid postId)
        {
            try
            {
                var post = await _dbContext.Posts
                 .FirstOrDefaultAsync(p => p.PostId == postId);

                return post;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postId} not readed");
                throw new PostDbOperationNotExecutedException(ex, $"Post id {postId}");
            }
        }

        public async Task UpdatePostAsync(PostEntity post)
        {
            try
            {
                _dbContext.Posts.Update(post);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {post.PostId} not updated");
                throw new PostDbOperationNotExecutedException(ex, $"Post id {post.PostId}");
            }
        }

        public async Task<Guid> DeletePostAsync(Guid postId)
        {
            try
            {
                var postToDelete = await _dbContext.Posts.FirstOrDefaultAsync(p => p.PostId == postId);

                var authorId = postToDelete.AuthorId;

                _dbContext.Posts.Remove(postToDelete);

                await _dbContext.SaveChangesAsync();

                return authorId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postId} not deleted");
                throw new PostDbOperationNotExecutedException(ex, $"Post id {postId}");
            }
        }

        public async Task DeleteAllPostsByAuthorIdAsync(Guid authorId)
        {
            try
            {
                var postsToDelete = await _dbContext.Posts
                    .Where(p => p.AuthorId == authorId)
                    .ToListAsync();

                _dbContext.RemoveRange(postsToDelete);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"All post by author {authorId} not deleted");
                throw new PostDbOperationNotExecutedException(ex, $"Author id {authorId}");
            }
        }
    }
}