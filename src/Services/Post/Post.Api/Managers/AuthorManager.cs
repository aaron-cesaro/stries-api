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

            try
            {
                var authorCheck = await _authorRepository.ReadAuthorAsync(authorToCreate.Id);

                if (authorCheck == null)
                {
                    throw new AuthorNotProcessedException();
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

                await _authorRepository.InsertAuthorAsync(author);

                Log.Information($"Author id {author.AuthorId} successfully created");

                return author.AuthorId;
            }
            catch (AuthorNotProcessedException ex)
            {
                Log.Error($"Author id {authorToCreate.Id} cannot be created because author is already present");
                throw new AuthorNotProcessedException(ex, $"Author id {authorToCreate.Id}");
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
                    throw new AuthorNotFoundException();
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
            catch (AuthorNotFoundException ex)
            {
                Log.Information($"Author with id {authorId} not found");
                throw new AuthorNotFoundException(ex, $"Author id {authorId}");
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
                    throw new AuthorNotFoundException();
                }

                authorToUpdate.UpdatedAt = DateTime.UtcNow;

                await _authorRepository.UpdateAuthorAsync(authorToUpdate);

                Log.Information($"Author with Id {author.Id} successfully saved");
            }
            catch (AuthorNotFoundException ex)
            {
                Log.Information($"Author with id {author.Id} not found");
                throw new AuthorNotFoundException(ex, $"Author id {author.Id}");
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
                    throw new AuthorNotFoundException();
                }

                await _authorRepository.DeleteAuthorAsync(authorId);

                Log.Information($"Author with Id {authorId} successfully deleted");

            }
            catch (AuthorNotFoundException ex)
            {
                Log.Information($"Author with id {authorId} not found");
                throw new AuthorNotFoundException(ex, $"Author id {authorId}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author with id {authorId} cannot be deleted");
                throw new AuthNotProcessedException(ex, $"Author id {authorId}");
            }
        }
    }
}
