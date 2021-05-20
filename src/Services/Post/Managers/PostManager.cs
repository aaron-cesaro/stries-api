using Newtonsoft.Json;
using Post.Application.EventHandlers.Events;
using Post.Application.Events;
using Post.Application.Models;
using Post.Database.Entities;
using Post.Database.Models;
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

        public async Task<Guid> CreatePostAsync(CreatePostRequest postToCreate)
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
                    AuthorId = postToCreate.AuthorId,
                    Title = postToCreate.Title,
                    ImageUrl = postToCreate.ImageUrl,
                    CreatedAt = creationDate
                };

                // Send post creation event to message broker
                _publisher.Publish(
                    JsonConvert.SerializeObject(postCreatedEvent),
                    MessageBrokerHelpers.SetMessageRoute("Post", "Created"));

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with Author id {post.AuthorId} cannot be created");
            }

            Log.Information($"Post with Id {postId} and Author id {post.AuthorId} successfully created");

            return postId;
        }

        public async Task<bool> SaveAndPusblishPostAsync(Guid postId, string status, PostData newPostData)
        {
            Log.Information($"Saving Post with id {postId}");

            var updated = false;

            try
            {
                var postToUpdate = await _postRepository.ReadPostAsync(postId);

                if(postToUpdate == null)
                {
                    Log.Information($"Post with id {postId} not found");

                    return updated;
                }

                postToUpdate.Body = new PostBody
                {
                    CompanyDescription = newPostData.CompanyDescription,
                    CompanyThesis = newPostData.CompanyThesis,
                    CompanyValuation = newPostData.CompanyValuation,
                    CompanyFinancials = newPostData.CompanyFinancials,
                    CompanyOther = newPostData.CompanyOther
                };

                postToUpdate.UpdatedAt = DateTime.UtcNow;

                var postStatus = status == "published" ? PostStatus.published : PostStatus.draft;

                if (postToUpdate.Status != PostStatus.published && postToUpdate.Status == postStatus)
                {
                    Log.Information($"Post with Id {postId} is going to be published");
                    postToUpdate.Status = PostStatus.published;
                }

                await _postRepository.UpdatePostAsync(postToUpdate);

                if (postToUpdate.Status == PostStatus.published)
                {
                    var postPublishedEvent = new PostPublishedEvent
                    {
                        PostId = postId,
                        Title = postToUpdate.Title,
                        Summary = postToUpdate.Summary,
                        ImageUrl = postToUpdate.ImageUrl,
                        AuthorId = postToUpdate.AuthorId,
                        PublishedAt = postToUpdate.PublishedAt
                    };

                    // Send post published event to message broker
                    _publisher.Publish(
                        JsonConvert.SerializeObject(postPublishedEvent),
                        MessageBrokerHelpers.SetMessageRoute("Post", "Published"));
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be saved");
            }

            Log.Information($"Post with Id {postId} successfully saved with status {status}");

            return updated;
        }

        public async Task RemovePostAsync(Guid postId)
        {
            Log.Information($"Deleting Post with id {postId}");

            try
            {
                var authorId = await _postRepository.DeletePostAsync(postId);

                var postDeletedEvent = new PostDeletedEvent
                {
                    PostId = postId,
                    AuthorId = authorId
                };

                // Send post deletion event to message broker
                _publisher.Publish(
                    JsonConvert.SerializeObject(postDeletedEvent),
                    MessageBrokerHelpers.SetMessageRoute("Post", "Deleted"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be deleted");
            }

            Log.Information($"Post with Id {postId} successfully deleted");
        }
    }
}
