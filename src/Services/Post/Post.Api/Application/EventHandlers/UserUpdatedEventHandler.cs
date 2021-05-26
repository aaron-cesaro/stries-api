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
    public class UserUpdatedEventHandler : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IAuthorManager _authorManager;

        public UserUpdatedEventHandler(ISubscriber subscriber, IAuthorManager authorManager)
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
            if (header.Keys.Contains("user") && header.Values.Contains("updated"))
            {
                var response = JsonConvert.DeserializeObject<UserUpdatedEvent>(message);

                try
                {
                    var authorToUpdate = new AuthorCreateRequest
                    {
                        Id = response.Id,
                        FirstName = response.FirstName,
                        LastName = response.LastName,
                        NickName = response.NickName,
                        EmailAddress = response.EmailAddress,
                        Biography = response.Biography
                    };

                    await _authorManager.UpdateAuthorAsync(authorToUpdate);
                }
                catch (Exception)
                {
                    Log.Information($"Author with email {response.EmailAddress} cannot be updated");
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
