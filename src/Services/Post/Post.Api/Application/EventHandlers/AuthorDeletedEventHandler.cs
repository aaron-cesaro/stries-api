using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Post.Api.Application.Events;
using Post.Api.Infrastructure.MessageBroker;
using Post.Api.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Post.Api.Application.EventHandlers
{
    public class AuthorDeletedEventHandler : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IPostManager _postManager;
        private readonly IAuthorManager _authorManager;

        public AuthorDeletedEventHandler(ISubscriber subscriber, IPostManager postManager, IAuthorManager authorManager)
        {
            _subscriber = subscriber;
            _postManager = postManager;
            _authorManager = authorManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.SubscribeAsync(Subscribe);
            return Task.CompletedTask;
        }

        private async Task<bool> Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<AuthorDeletedEvent>(message);

            try
            {
                await _postManager.DeleteAllPostsByAuthorIdAsync(response.Id);

                await _authorManager.DeleteAuthorByIdAsync(response.Id);
            }
            catch (Exception)
            {
                Log.Information($"Author with id {response.Id} cannot be deleted");
                return false;
            }

            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
