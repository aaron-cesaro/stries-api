﻿using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
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
    public class AuthorCreatedEventHandler : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IAuthorManager _authorManager;

        public AuthorCreatedEventHandler(ISubscriber subscriber, IAuthorManager authorManager)
        {
            _subscriber = subscriber;
            _authorManager = authorManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.SubscribeAsync(Subscribe);
            return Task.CompletedTask;
        }

        private async Task<bool> Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<AuthorCreateRequest>(message);

            try
            {
                var authorId = await _authorManager.CreateAuthorAsync(response);
            }
            catch (Exception)
            {
                Log.Information($"Author with id {response.Id} cannot be created");
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
