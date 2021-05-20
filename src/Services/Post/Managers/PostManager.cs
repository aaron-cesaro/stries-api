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
                Url = postToCreate.Url,
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
                    Url = postToCreate.Url,
                    CreatedAt = creationDate
                };

                // Send async event to message broker
                _publisher.Publish(
                    JsonConvert.SerializeObject(postCreatedEvent),
                    MessageBrokerHelpers.SetMessageRoute("Post", "Created"));

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with Author id {post.AuthorId} encounter an error");
            }

            Log.Information($"Post with Id {postId} and Author id {post.AuthorId} successfully created");

            return postId;
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

                // Send async event to message broker
                _publisher.Publish(
                    JsonConvert.SerializeObject(postDeletedEvent),
                    MessageBrokerHelpers.SetMessageRoute("Post", "Deleted"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} encounter an error");
            }

            Log.Information($"Post with Id {postId} successfully deleted");
        }
    }
}
