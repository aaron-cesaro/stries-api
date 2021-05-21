using Microsoft.EntityFrameworkCore;
using Post.Database.Contextes;
using Post.Database.Entities;
using Post.Infrastructure.Exceptions;
using Post.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Post.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly PostContext _postContext;

        public AuthorRepository(PostContext postContext)
        {
            _postContext = postContext ?? throw new ArgumentNullException(nameof(postContext));
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
