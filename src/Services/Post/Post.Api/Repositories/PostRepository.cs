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
            var postId = Guid.NewGuid();

            try
            {
                post.PostId = postId;

                _dbContext.Posts.Add(post);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postId} not inserted");
                throw new PostDbOperationNotExecutedException(ex, $"Post id {postId}");
            }

            return postId;
        }

        public async Task<PostEntity> ReadPostByIdAsync(Guid postId)
        {
            PostEntity post = null;

            try
            {
                post = await _dbContext.Posts
                 .FirstOrDefaultAsync(p => p.PostId == postId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postId} not readed");
                throw new PostDbOperationNotExecutedException(ex, $"Post id {postId}");
            }

            return post;
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
            var authorId = Guid.Empty;

            try
            {
                var postToDelete = await _dbContext.Posts.FirstOrDefaultAsync(p => p.PostId == postId);

                authorId = postToDelete.AuthorId;

                _dbContext.Posts.Remove(postToDelete);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postId} not deleted");
                throw new PostDbOperationNotExecutedException(ex, $"Post id {postId}");
            }

            return authorId;
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

        public async Task InsertAuthorAsync(AuthorEntity author)
        {
            try
            {
                _dbContext.Authors.Add(author);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author {author.AuthorId} not inserted");
                throw new PostDbOperationNotExecutedException(ex, $"Author id {author.AuthorId}");
            }
        }

        public async Task<AuthorEntity> ReadAuthorAsync(Guid authorId)
        {
            AuthorEntity author = null;

            try
            {
                author = await _dbContext.Authors
                 .FirstOrDefaultAsync(a => a.AuthorId == authorId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author {authorId} not readed");
                throw new PostDbOperationNotExecutedException(ex, $"Author id {authorId}");
            }

            return author;
        }

        public async Task UpdateAuthorAsync(AuthorEntity author)
        {
            try
            {
                _dbContext.Authors.Update(author);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author {author.AuthorId} not updated");
                throw new PostDbOperationNotExecutedException(ex, $"Author id {author.AuthorId}");
            }
        }

        public async Task DeleteAuthorAsync(Guid authorId)
        {
            try
            {
                var authorToDelete = await _dbContext.Authors
                    .FirstOrDefaultAsync(a => a.AuthorId == authorId);

                _dbContext.Authors.Remove(authorToDelete);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author {authorId} not deleted");
                throw new PostDbOperationNotExecutedException(ex, $"Author id {authorId}");
            }
        }
    }
}