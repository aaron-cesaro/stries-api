using Post.Api.Application.Models;
using Post.Api.Database.Entities;
using Post.Api.Infrastructure.Exceptions;
using Post.Api.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Post.Api.Managers
{
    public class AuthorManager : IAuthorManager
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorManager(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
        }

        public async Task<Guid> CreateAuthorAsync(AuthorCreateRequest authorToCreate)
        {
            Log.Information($"Creating Author Nickname {authorToCreate.NickName}");

            var authorCheck = await _authorRepository.ReadAuthorAsync(authorToCreate.Id);

            if (authorCheck == null)
            {
                Log.Error($"Author id {authorToCreate.Id} cannot be created because author is already present");
                throw new AuthorNotProcessedException($"Author id {authorToCreate.Id}");
            }

            var creationDate = DateTime.UtcNow;

            var author = new AuthorEntity
            {
                AuthorId = authorToCreate.Id,
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

                Log.Information($"Author id {author.AuthorId} successfully created");

                return author.AuthorId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author id {authorToCreate.Id} cannot be created");
                throw new AuthNotProcessedException(ex, $"Author id {authorToCreate.Id}");
            }
        }

        public async Task<AuthorResponse> GetAuthorByIdAsync(Guid authorId)
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
                    Id = authorToGet.AuthorId,
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

        public async Task DeleteAuthorByIdAsync(Guid authorId)
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

                await _authorRepository.DeleteAuthorAsync(authorId);

                Log.Information($"Author with Id {authorId} successfully deleted");

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author with id {authorId} cannot be deleted");
                throw new AuthNotProcessedException(ex, $"Author id {authorId}");
            }
        }
    }
}
