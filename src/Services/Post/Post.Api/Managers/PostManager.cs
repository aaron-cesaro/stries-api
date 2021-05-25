using Newtonsoft.Json;
using Post.Api.Application.EventHandlers.Events;
using Post.Api.Application.Events;
using Post.Api.Application.Models;
using Post.Api.Database.Entities;
using Post.Api.Infrastructure.Exceptions;
using Post.Api.Infrastructure.MessageBroker;
using Post.Api.Infrastructure.MessageBroker.Helpers;
using Post.Api.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Post.Api.Managers
{
    public class PostManager : IPostManager
    {
        private readonly IPostRepository _postRepository;
        private readonly IPublisher _publisher;

        public PostManager(IPostRepository postRepository, IPublisher publisher)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task<Guid> CreatePostAsync(PostCreateRequest postToCreate)
        {
            Log.Information($"Creating Post with Author id {postToCreate.AuthorId}");

            var creationDate = DateTime.UtcNow;

            var postId = Guid.Empty;

            var post = new PostEntity
            {
                AuthorId = postToCreate.AuthorId,
                Title = postToCreate.Title,
                Summary = postToCreate.Summary,
                ImageUrl = postToCreate.ImageUrl,
                Status = PostStatus.draft,
                Body = new PostBody
                {
                    CompanyDescription = postToCreate.PostData.CompanyDescription,
                    CompanyThesis = postToCreate.PostData.CompanyThesis,
                    CompanyFinancials = postToCreate.PostData.CompanyFinancials,
                    CompanyValuation = postToCreate.PostData.CompanyValuation,
                    CompanyOther = postToCreate.PostData.CompanyOther
                },
                Rating = 0,
                RatingVoters = 0,
                CreatedAt = creationDate,
                UpdatedAt = creationDate
            };

            try
            {
                var authorIsPresent = await AuthorIsPresentAsync(postToCreate.AuthorId);

                if (!authorIsPresent)
                {
                    Log.Error($"Post with Author id {postToCreate.AuthorId} cannot be created because author was not found");
                    return postId;
                }

                postId = await _postRepository.InsertPostAsync(post);

                var postCreatedEvent = new PostCreatedEvent
                {
                    AuthorId = postToCreate.AuthorId,
                    Title = postToCreate.Title,
                    ImageUrl = postToCreate.ImageUrl,
                    CreatedAt = creationDate
                };

                // Send post creation event through message broker
                _publisher.Publish(
                    JsonConvert.SerializeObject(postCreatedEvent),
                    MessageBrokerHelpers.SetMessageRoute("PostCreated", "PostCreated"));

                Log.Information($"Post with Id {postId} and Author id {post.AuthorId} successfully created");

                return postId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with Author id {post.AuthorId} cannot be created");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
            }
        }

        public async Task<PostReponse> GetPostByIdAsync(Guid postId)
        {
            Log.Information($"Retrieving Post with id {postId}");

            try
            {
                var postToGet = await _postRepository.ReadPostByIdAsync(postId);

                if (postToGet == null)
                {
                    Log.Information($"Post with id {postId} not found");
                    throw new PostNotFoundException($"Post id {postId}");
                }

                var postAuthor = await GetAuthorByIdAsync(postToGet.AuthorId);

                var post = new PostReponse
                {
                    Id = postToGet.PostId,
                    Author = postAuthor,
                    Title = postToGet.Title,
                    Summary = postToGet.Summary,
                    ImageUrl = postToGet.ImageUrl,
                    Status = PostStatus.draft,
                    PostData = new PostData
                    {
                        CompanyDescription = postToGet.Body.CompanyDescription,
                        CompanyThesis = postToGet.Body.CompanyThesis,
                        CompanyFinancials = postToGet.Body.CompanyFinancials,
                        CompanyValuation = postToGet.Body.CompanyValuation,
                        CompanyOther = postToGet.Body.CompanyOther
                    },
                    Rating = postToGet.Rating,
                    RatingVoters = postToGet.RatingVoters,
                    CreatedAt = postToGet.CreatedAt,
                    UpdatedAt = postToGet.UpdatedAt,
                    PublishedAt = postToGet.PublishedAt
                };

                Log.Information($"Post with Id {postId} successfully retrieved");

                return post;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be retrieved");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
            }
        }

        public async Task SavePostAsync(Guid postId, PostData newPostData)
        {
            Log.Information($"Saving Post with id {postId}");

            try
            {
                var postToSave = await _postRepository.ReadPostByIdAsync(postId);

                if (postToSave == null)
                {
                    Log.Information($"Post with id {postId} not found");
                    throw new PostNotFoundException($"Post id {postId}");
                }

                if (postToSave.Status == PostStatus.published)
                {
                    Log.Information($"Post with id {postId} cannot be saved because it's already published");
                    throw new PostAlreadyPublishedException($"Post id {postId}");
                }

                var authorIsPresent = await AuthorIsPresentAsync(postToSave.AuthorId);

                if (!authorIsPresent)
                {
                    Log.Error($"Post with Author id {postToSave.AuthorId} cannot be saved because author was not found");
                    throw new PostNotProcessedException($"Post id {postId}");
                }

                postToSave.Body = new PostBody
                {
                    CompanyDescription = newPostData.CompanyDescription,
                    CompanyThesis = newPostData.CompanyThesis,
                    CompanyValuation = newPostData.CompanyValuation,
                    CompanyFinancials = newPostData.CompanyFinancials,
                    CompanyOther = newPostData.CompanyOther
                };

                postToSave.UpdatedAt = DateTime.UtcNow;

                await _postRepository.UpdatePostAsync(postToSave);

                Log.Information($"Post with Id {postId} successfully saved");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be saved");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
            }
        }

        public async Task PublishPostAsync(Guid postId)
        {
            Log.Information($"Publishing Post id {postId}");

            try
            {
                var postToPublish = await _postRepository.ReadPostByIdAsync(postId);

                if (postToPublish == null)
                {
                    Log.Information($"Post with id {postId} not found");
                    throw new PostNotFoundException($"Post id {postId}");
                }

                if (postToPublish.Status == PostStatus.published)
                {
                    Log.Information($"Post with id {postId} cannot be published because it's already published");
                    throw new PostAlreadyPublishedException($"Post id {postToPublish.PostId}");
                }

                var authorIsPresent = await AuthorIsPresentAsync(postToPublish.AuthorId);

                if (!authorIsPresent)
                {
                    Log.Error($"Post with Author id {postToPublish.AuthorId} cannot be published because author was not found");
                    throw new PostNotProcessedException($"Post id {postId}");
                }

                // Update post status
                postToPublish.Status = PostStatus.published;

                var publishedDate = DateTime.UtcNow;

                postToPublish.UpdatedAt = publishedDate;
                postToPublish.PublishedAt = publishedDate;

                await _postRepository.UpdatePostAsync(postToPublish);

                // Create publish event for message propagation
                var postPublishedEvent = new PostPublishedEvent
                {
                    PostId = postId,
                    Title = postToPublish.Title,
                    Summary = postToPublish.Summary,
                    ImageUrl = postToPublish.ImageUrl,
                    AuthorId = postToPublish.AuthorId,
                    PublishedAt = postToPublish.PublishedAt
                };

                // Send post published event through message broker
                _publisher.Publish(
                    JsonConvert.SerializeObject(postPublishedEvent),
                    MessageBrokerHelpers.SetMessageRoute("PostPublished", "PostPublished"));

                Log.Information($"Post with Id {postId} successfully published at {publishedDate}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be published");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
            }
        }

        public async Task ArchivePostAsync(Guid postId)
        {
            Log.Information($"Archiving Post id {postId}");

            try
            {
                var postToArchive = await _postRepository.ReadPostByIdAsync(postId);

                if (postToArchive == null)
                {
                    Log.Information($"Post with id {postId} not found");
                    throw new PostNotFoundException($"Post id {postId}");
                }

                if (postToArchive.Status == PostStatus.archived)
                {
                    Log.Information($"Post with id {postId} cannot be archived because it's already archived");
                    throw new PostAlreadyArchivedException($"Post id {postToArchive.PostId}");
                }

                var authorIsPresent = await AuthorIsPresentAsync(postToArchive.AuthorId);

                if (!authorIsPresent)
                {
                    Log.Error($"Post with Author id {postToArchive.AuthorId} cannot be archived because author was not found");
                    throw new PostNotProcessedException($"Post id {postId}");
                }

                // Update post status
                postToArchive.Status = PostStatus.archived;

                var archivedDate = DateTime.UtcNow;

                postToArchive.UpdatedAt = archivedDate;

                postToArchive.ArchivedAt = archivedDate;

                await _postRepository.UpdatePostAsync(postToArchive);

                // Create publish event for message propagation
                var postArchivedEvent = new PostArchivedEvent
                {
                    PostId = postId,
                    Title = postToArchive.Title,
                    Summary = postToArchive.Summary,
                    ImageUrl = postToArchive.ImageUrl,
                    AuthorId = postToArchive.AuthorId,
                    ArchivedAt = postToArchive.ArchivedAt
                };

                // Send post published event through message broker
                _publisher.Publish(
                    JsonConvert.SerializeObject(postArchivedEvent),
                    MessageBrokerHelpers.SetMessageRoute("PostArchived", "PostArchived"));

                Log.Information($"Post with Id {postId} successfully archived at {archivedDate}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be archived");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
            }
        }

        public async Task RemovePostByIdAsync(Guid postId)
        {
            Log.Information($"Deleting Post with id {postId}");

            try
            {
                var postToDelete = await _postRepository.ReadPostByIdAsync(postId);

                if (postToDelete == null)
                {
                    Log.Information($"Post with id {postId} not found");
                    throw new PostNotFoundException($"Post id {postId}");

                }

                var authorId = await _postRepository.DeletePostAsync(postId);

                var postDeletedEvent = new PostDeletedEvent
                {
                    PostId = postId,
                    AuthorId = authorId
                };

                // Send post deletion event through message broker
                _publisher.Publish(
                    JsonConvert.SerializeObject(postDeletedEvent),
                    MessageBrokerHelpers.SetMessageRoute("PostDeleted", "PostDeleted"));

                Log.Information($"Post with Id {postId} successfully deleted");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be deleted");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
            }
        }

        public async Task RemoveAllPostsByAuthorIdAsync(Guid authorId)
        {
            Log.Information($"Deleting all Post by Author id {authorId}");

            try
            {
                await _postRepository.DeleteAllPostsByAuthorIdAsync(authorId);

                Log.Information($"All posts by author id {authorId} successfully deleted");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"All posts by author id {authorId} cannot be deleted");
                throw new PostNotProcessedException(ex, $"Author id {authorId}");
            }
        }

        public async Task<Guid> CreateAuthorAsync(AuthorCreateRequest authorToCreate)
        {
            Log.Information($"Creating Author Nickname {authorToCreate.NickName}");

            var authorIsPresent = await AuthorIsPresentAsync(authorToCreate.Id);

            if (authorIsPresent)
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

                await _postRepository.InsertAuthorAsync(author);

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
                var authorToGet = await _postRepository.ReadAuthorAsync(authorId);

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
                var authorToUpdate = await _postRepository.ReadAuthorAsync(author.Id);

                if (authorToUpdate == null)
                {
                    Log.Information($"Author with id {author.Id} not found");
                    throw new AuthorNotFoundException($"Author id {author.Id}");
                }

                authorToUpdate.UpdatedAt = DateTime.UtcNow;

                await _postRepository.UpdateAuthorAsync(authorToUpdate);

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
                var authorToDelete = await _postRepository.ReadAuthorAsync(authorId);

                if (authorToDelete == null)
                {
                    Log.Information($"Author with id {authorId} not found");
                    throw new AuthorNotFoundException($"Author id {authorId}");

                }

                await RemoveAllPostsByAuthorIdAsync(authorId);

                await _postRepository.DeleteAuthorAsync(authorId);

                Log.Information($"Author with Id {authorId} successfully deleted");

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author with id {authorId} cannot be deleted");
                throw new AuthNotProcessedException(ex, $"Author id {authorId}");
            }
        }

        public async Task<bool> AuthorIsPresentAsync(Guid authorId)
        {
            Log.Information($"Search Author with id {authorId}");

            var isPresent = false;

            try
            {
                var authorToGet = await _postRepository.ReadAuthorAsync(authorId);

                if (authorToGet != null)
                {
                    isPresent = true;

                    Log.Information($"Author with Id {authorId} not found");
                }

                return isPresent;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Author with id {authorId} cannot be searched");
                throw new AuthorNotProcessedException(ex, $"Author id {authorId}");
            }
        }
    }
}
