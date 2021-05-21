using Post.Application.Models;
using Post.Database.Entities;
using Post.Infrastructure.Exceptions;
using Post.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Post.Managers
{
    public class AuthorManager : IAuthorManager
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IPostManager _postManager;

        public AuthorManager(IAuthorRepository authorRepository, IPostManager postManager)
        {
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            _postManager = postManager ?? throw new ArgumentNullException(nameof(postManager));
        }

        public async Task<Guid> CreateAuthorAsync(AuthorCreateRequest authorToCreate)
        {
            Log.Information($"Creating Author Nickname {authorToCreate.NickName}");

            var creationDate = DateTime.UtcNow;

            var author = new AuthorEntity
            {
                Id = authorToCreate.Id,
                FirstName = authorToCreate.FirstName,
                LastName = authorToCreate.LastName,
                NickName = authorToCreate.NickName,
                EmailAddress = authorToCreate.EmailAddress,
                Biography = authorToCreate.Biography,
                PostPublished = 0,
                CreatedAt = creationDate,
                UpdatedAt = creationDate
            };

            try
            {

                await _authorRepository.InsertAuthorAsync(author);

                Log.Information($"Author id {author.Id} successfully created");

                return author.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author id {authorToCreate.Id} cannot be created");
                throw new AuthNotProcessedException(ex, $"Author id {authorToCreate.Id}");
            }
        }

        public async Task<AuthorResponse> ReadAuthorByIdAsync(Guid authorId)
        {
            Log.Information($"Retrieving Author with id {authorId}");

            try
            {
                var authorToGet = await _authorRepository.ReadAuthorAsync(authorId);

                if (authorToGet == null)
                {
                    Log.Information($"Author with id {authorId} not found");
                    throw new AuthorNotFoundException($"Author id {authorId}");
                }

                var author = new AuthorResponse
                {
                    Id = authorToGet.Id,
                    FirstName = authorToGet.FirstName,
                    LastName = authorToGet.LastName,
                    NickName = authorToGet.NickName,
                    EmailAddress = authorToGet.EmailAddress,
                    Biography = authorToGet.Biography,
                    CreatedAt = authorToGet.CreatedAt,
                    UpdatedAt = authorToGet.UpdatedAt
                };

                Log.Information($"Author with Id {authorId} successfully retrieved");

                return author;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author with id {authorId} cannot be retrieved");
                throw new AuthorNotProcessedException(ex, $"Author id {authorId}");
            }
        }

        public async Task UpdateAuthorAsync(AuthorCreateRequest author)
        {
            Log.Information($"Updating author with id {author.Id}");

            try
            {
                var authorToUpdate = await _authorRepository.ReadAuthorAsync(author.Id);

                if (authorToUpdate == null)
                {
                    Log.Information($"Author with id {author.Id} not found");
                    throw new AuthorNotFoundException($"Author id {author.Id}");
                }

                authorToUpdate.UpdatedAt = DateTime.UtcNow;

                await _authorRepository.UpdateAuthorAsync(authorToUpdate);

                Log.Information($"Author with Id {author.Id} successfully saved");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author with id {author.Id} cannot be saved");
                throw new AuthorNotProcessedException(ex, $"Author id {author.Id}");
            }
        }

        public async Task<bool> DeleteAuthorAsync(Guid authorId)
        {
            Log.Information($"Deleting author with id {authorId}");

            try
            {
                var authorToDelete = await _authorRepository.ReadAuthorAsync(authorId);

                if (authorToDelete == null)
                {
                    Log.Information($"Author with id {authorId} not found");
                    throw new AuthorNotFoundException($"Author id {authorId}");

                }

                await _postManager.RemoveAllPostsByAuthorAsync(authorId);

                await _authorRepository.DeleteAuthorAsync(authorId);

                Log.Information($"Author with Id {authorId} successfully deleted");

                return true;

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author with id {authorId} cannot be deleted");
                throw new PostNotProcessedException(ex, $"Author id {authorId}");
            }
        }
    }
}
