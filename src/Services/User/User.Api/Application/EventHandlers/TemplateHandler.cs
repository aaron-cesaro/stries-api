using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using User.Api.Application.EventHandlers.Events;
using User.Api.Infrastructure.MessageBroker;
using User.Api.Interfaces;

namespace User.Api.Application.EventHandlers
{
    public class TemplateHandler : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly ITemplateManager _templateManager;

        public TemplateHandler(ISubscriber subscriber, ITemplateManager templateManager)
        {
            _subscriber = subscriber;
            _templateManager = templateManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(ProcessMessage);
            return Task.CompletedTask;
        }

        private bool ProcessMessage(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<TemplateEvent>(message);

            /*
            if (response.IsDeletable)
            {
                // templateManager.//method(response.WeatherId).GetAwaiter().GetResult();
            }
            */
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
