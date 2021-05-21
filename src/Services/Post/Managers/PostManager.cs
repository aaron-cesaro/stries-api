using Newtonsoft.Json;
using Post.Application.EventHandlers.Events;
using Post.Application.Events;
using Post.Application.Models;
using Post.Database.Entities;
using Post.Database.Models;
using Post.Infrastructure.Exceptions;
using Post.Infrastructure.MessageBroker;
using Post.Infrastructure.MessageBroker.Helpers;
using Post.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Post.Managers
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

            var postId = Guid.Empty;

            try
            {

                postId = await _postRepository.InsertPostAsync(post);

                var postCreatedEvent = new PostCreatedEvent
                {
                    PostCreated_AuthorId = postToCreate.AuthorId,
                    PostCreated_Title = postToCreate.Title,
                    PostCreated_ImageUrl = postToCreate.ImageUrl,
                    PostCreated_CreatedAt = creationDate
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
                throw new PostNotProcessedException(ex, "Post id {postId}");
            }           
        }

        public async Task<PostReponse> GetPostByIdAsync(Guid postId)
        {
            Log.Information($"Retrieving Post with id {postId}");

            try
            {
                var postToGet = await _postRepository.ReadPostAsync(postId);

                if (postToGet == null)
                {
                    Log.Information($"Post with id {postId} not found");
                    throw new PostNotFoundException($"Post id {postId}");
                }

                var post = new PostReponse
                {
                    Id = postToGet.Id,
                    AuthorId = postToGet.AuthorId,
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
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be retrieved");
                throw new PostNotProcessedException(ex, "Post id {postId}");
            }
        }

        public async Task SavePostAsync(Guid postId, PostData newPostData)
        {
            Log.Information($"Saving Post with id {postId}");

            try
            {
                var postToSave = await _postRepository.ReadPostAsync(postId);

                if(postToSave == null)
                {
                    Log.Information($"Post with id {postId} not found");
                    throw new PostNotFoundException($"Post id {postId}");
                }

                if (postToSave.Status == PostStatus.published)
                {
                    Log.Information($"Post with id {postId} cannot be saved because it's already published");
                    throw new PostAlreadyPublishedException($"Post id {postId}");
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
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be saved");
                throw new PostNotProcessedException(ex, "Post id {postId}");
            }

            Log.Information($"Post with Id {postId} successfully saved");
        }

        public async Task PublishPostAsync(Guid postId)
        {
            Log.Information($"Publishing Post with id {postId}");

            try
            {
                var postToPublish = await _postRepository.ReadPostAsync(postId);

                if (postToPublish == null)
                {
                    Log.Information($"Post with id {postId} not found");
                    throw new PostNotFoundException($"Post id {postId}");
                }

                if(postToPublish.Status == PostStatus.published)
                {
                    Log.Information($"Post with id {postId} cannot be published because it's already published");
                    throw new PostAlreadyPublishedException($"Post id {postToPublish.Id}");
                }

                var publishedDate = DateTime.UtcNow;

                postToPublish.UpdatedAt = publishedDate;
                postToPublish.PublishedAt = publishedDate;

                // Update post status
                postToPublish.Status = PostStatus.published;

                await _postRepository.UpdatePostAsync(postToPublish);

                // Create publish event for message propagation
                var postPublishedEvent = new PostPublishedEvent
                {
                    PostPublished_PostId = postId,
                    PostPublished_Title = postToPublish.Title,
                    PostPublished_Summary = postToPublish.Summary,
                    PostPublished_ImageUrl = postToPublish.ImageUrl,
                    PostPublished_AuthorId = postToPublish.AuthorId,
                    PostPublished_PublishedAt = postToPublish.PublishedAt
                };

                // Send post published event through message broker
                _publisher.Publish(
                    JsonConvert.SerializeObject(postPublishedEvent),
                    MessageBrokerHelpers.SetMessageRoute("PostPublished", "PostPublished"));

                Log.Information($"Post with Id {postId} successfully published at {publishedDate}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be saved");
                throw new PostNotProcessedException(ex, "Post id {postId}");
            }
        }

        public async Task RemovePostAsync(Guid postId)
        {
            Log.Information($"Deleting Post with id {postId}");

            try
            {
                var postToDelete = await _postRepository.ReadPostAsync(postId);

                if(postToDelete == null)
                {
                    Log.Information($"Post with id {postId} not found");
                    throw new PostNotFoundException($"Post id {postId}");

                }

                var authorId = await _postRepository.DeletePostAsync(postId);

                var postDeletedEvent = new PostDeletedEvent
                {
                    PostDeleted_PostId = postId,
                    PostDeleted_AuthorId = authorId
                };

                // Send post deletion event through message broker
                _publisher.Publish(
                    JsonConvert.SerializeObject(postDeletedEvent),
                    MessageBrokerHelpers.SetMessageRoute("PostDeleted", "PostDeleted"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be deleted");
                throw new PostNotProcessedException(ex, "Post id {postId}");
            }

            Log.Information($"Post with Id {postId} successfully deleted");
        }
    }
}
