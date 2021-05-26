using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Post.Api.Application.Events;
using Post.Api.Application.Models;
using Post.Api.Infrastructure.MessageBroker;
using Post.Api.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Post.Api.Application.EventHandlers
{
    public class UserCreatedEventHandler : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IAuthorManager _authorManager;

        public UserCreatedEventHandler(ISubscriber subscriber, IAuthorManager authorManager)
        {
            _subscriber = subscriber;
            _authorManager = authorManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.SubscribeAsync(ProcessMessage);
            return Task.CompletedTask;
        }

        private async Task<bool> ProcessMessage(string message, IDictionary<string, object> header)
        {
            if (header.Keys.Contains("user") && header.Values.Contains("created"))
            {
                var response = JsonConvert.DeserializeObject<UserCreatedEvent>(message);

                try
                {
                    var authorToCreate = new AuthorCreateRequest
                    {
                        Id = response.Id,
                        FirstName = response.FirstName,
                        LastName = response.LastName,
                        NickName = response.NickName,
                        EmailAddress = response.EmailAddress,
                        Biography = response.Biography
                    };

                    var authorId = await _authorManager.CreateAuthorAsync(authorToCreate);
                }
                catch (Exception)
                {
                    Log.Information($"Author with id {response.Id} cannot be created");
                    return false;
                }
            }

            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
