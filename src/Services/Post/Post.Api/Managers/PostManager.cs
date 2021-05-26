using Newtonsoft.Json;
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
        private readonly IAuthorManager _authorManager;
        private readonly IPublisher _publisher;

        public PostManager(IPostRepository postRepository, IAuthorManager authorManager, IPublisher publisher)
        {
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            _authorManager = authorManager ?? throw new ArgumentNullException(nameof(authorManager));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task<Guid> CreatePostAsync(PostCreateRequest postToCreate)
        {
            Log.Information($"Creating Post with Author id {postToCreate.AuthorId}");

            var creationDate = DateTime.UtcNow;

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
                var autorCheck = await _authorManager.GetAuthorByIdAsync(postToCreate.AuthorId);

                var postId = await _postRepository.InsertPostAsync(post);

                Log.Information($"Post with Id {postId} and Author id {post.AuthorId} successfully created");

                return postId;

            }
            catch (AuthorNotFoundException ex)
            {
                Log.Error($"Post with Author id {postToCreate.AuthorId} cannot be created because author was not found");
                throw new PostNotProcessedException(ex, $"Post title {postToCreate.Title}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with Author id {postToCreate.AuthorId} cannot be created");
                throw new PostNotProcessedException(ex, $"Post title {postToCreate.Title}");
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
                    throw new PostNotFoundException($"Post id {postId}");
                }

                var postAuthor = await _authorManager.GetAuthorByIdAsync(postToGet.AuthorId);

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
            catch (PostNotFoundException ex)
            {
                Log.Information($"Post with id {postId} not found");
                throw new PostNotFoundException(ex, $"Post id {postId}");
            }
            catch (AuthorNotFoundException ex)
            {
                Log.Error($"Post with id {postId} cannot be created because author was not found");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
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

                await _authorManager.GetAuthorByIdAsync(postToSave.AuthorId);

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
            catch (AuthorNotFoundException ex)
            {
                Log.Error($"Post with id {postId} cannot be saved because author was not found");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
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

                await _authorManager.GetAuthorByIdAsync(postToPublish.AuthorId);

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
                    AuthorId = postToPublish.AuthorId,
                    Title = postToPublish.Title,
                    Summary = postToPublish.Summary,
                    ImageUrl = postToPublish.ImageUrl,
                    PublishedAt = postToPublish.PublishedAt
                };

                // Send post published event through message broker
                _publisher.Publish(JsonConvert.SerializeObject(postPublishedEvent), MessageBrokerRoutingKeys.POST_PUBLISHED, null);

                Log.Information($"Post with Id {postId} successfully published at {publishedDate}");
            }
            catch (AuthorNotFoundException ex)
            {
                Log.Error($"Post with id {postId} cannot be published because author was not found");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
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

                var author = await _authorManager.GetAuthorByIdAsync(postToArchive.AuthorId);

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
                    AuthorId = author.Id,
                    Title = postToArchive.Title,
                    Summary = postToArchive.Summary,
                    ImageUrl = postToArchive.ImageUrl,
                    ArchivedAt = postToArchive.ArchivedAt
                };

                // Send post published event through message broker
                _publisher.Publish(JsonConvert.SerializeObject(postArchivedEvent), MessageBrokerRoutingKeys.POST_ARCHIVED, null);

                Log.Information($"Post with Id {postId} successfully archived at {archivedDate}");
            }
            catch (AuthorNotFoundException ex)
            {
                Log.Error($"Post with id {postId} cannot be archived because author was not found");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be archived");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
            }
        }

        public async Task DeletePostByIdAsync(Guid postId)
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
                _publisher.Publish(JsonConvert.SerializeObject(postDeletedEvent), MessageBrokerRoutingKeys.POST_DELETED, null);

                Log.Information($"Post with Id {postId} by author {authorId} successfully deleted");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be deleted");
                throw new PostNotProcessedException(ex, $"Post id {postId}");
            }
        }

        public async Task DeleteAllPostsByAuthorIdAsync(Guid authorId)
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
    }
}
