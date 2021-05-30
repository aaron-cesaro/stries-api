using Microsoft.EntityFrameworkCore;
using Post.Api.Database.Contextes;
using Post.Api.Database.Entities;
using Post.Api.Infrastructure.Exceptions;
using Post.Api.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Post.Api.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly PostContext _dbContext;

        public AuthorRepository(PostContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
                Log.Error(ex, ex.Message, $"Author {author.Id} not inserted");
                throw new PostDbOperationNotExecutedException(ex, $"Author id {author.Id}");
            }
        }

        public async Task<AuthorEntity> ReadAuthorAsync(Guid authorId)
        {
            AuthorEntity author = null;

            try
            {
                author = await _dbContext.Authors
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
                _dbContext.Authors.Update(author);

                await _dbContext.SaveChangesAsync();
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
                var authorToDelete = await _dbContext.Authors
                    .FirstOrDefaultAsync(a => a.Id == authorId);

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