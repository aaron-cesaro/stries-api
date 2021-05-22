using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Post.Application.EventHandlers.Events;
using Post.Application.Events;
using Post.Infrastructure.MessageBroker;
using Post.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Post.Application.EventHandlers
{
    public class AuthorDeletedEventHandler : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IPostManager _postManager;

        public AuthorDeletedEventHandler(ISubscriber subscriber, IPostManager postManager)
        {
            _subscriber = subscriber;
            _postManager = postManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<AuthorDeletedEvent>(message);

            try
            {
                var deleted = _postManager.DeleteAuthorAsync(response.Id);

                return true;
            }
            catch(Exception)
            {
                Log.Information($"Author with id {response.Id} cannot be deleted");
            }

            return false;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
