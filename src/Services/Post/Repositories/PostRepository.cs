using Microsoft.EntityFrameworkCore;
using Post.Database.Contextes;
using Post.Database.Entities;
using Post.Infrastructure.Exceptions;
using Post.Interfaces;
using Serilog;
using System;
using System.Linq;
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
                throw new PostDbOperationNotExecutedException(ex, $"Post id {postId}");
            }

            return postId;
        }

        public async Task<PostEntity> ReadPostByIdAsync(Guid postId)
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
                throw new PostDbOperationNotExecutedException(ex, $"Post id {postId}");
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
                throw new PostDbOperationNotExecutedException(ex, $"Post id {post.Id}");
            }
        }

        public async Task<Guid> DeletePostAsync(Guid postId)
        {
            var authorId = Guid.Empty;

            try
            {
                var postToDelete = await _postContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);

                authorId = postToDelete.AuthorId;

                _postContext.Posts.Remove(postToDelete);

                await _postContext.SaveChangesAsync();
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
                var postsToDelete = await _postContext.Posts
                    .Where(p => p.AuthorId == authorId)
                    .ToListAsync();

                _postContext.RemoveRange(postsToDelete);

                await _postContext.SaveChangesAsync();
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
                _postContext.Authors.Add(author);
                await _postContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author {author.Id} not inserted");
                throw new PostDbOperationNotExecutedException(ex, $"Author id {author.Id}");
            }
        }

        public async Task<AuthorEntity> ReadAuthorAsync(Guid authorId)
        {
            AuthorEntity author = null;

            try
            {
                author = await _postContext.Authors
                 .FirstOrDefaultAsync(a => a.Id == authorId);
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
                _postContext.Authors.Update(author);

                await _postContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author {author.Id} not updated");
                throw new PostDbOperationNotExecutedException(ex, $"Author id {author.Id}");
            }
        }

        public async Task DeleteAuthorAsync(Guid authorId)
        {
            try
            {
                var authorToDelete = await _postContext.Authors
                    .FirstOrDefaultAsync(a => a.Id == authorId);

                _postContext.Authors.Remove(authorToDelete);

                await _postContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author {authorId} not deleted");
                throw new PostDbOperationNotExecutedException(ex, $"Author id {authorId}");
            }
        }
    }
}