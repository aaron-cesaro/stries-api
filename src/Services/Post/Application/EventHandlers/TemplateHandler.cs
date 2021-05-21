using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Post.Application.EventHandlers.Events;
using Post.Infrastructure.MessageBroker;
using Post.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Post.Application.EventHandlers
{
    public class TemplateHandler : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IPostManager _postManage;

        public TemplateHandler(ISubscriber subscriber, IPostManager postManage)
        {
            _subscriber = subscriber;
            _postManage = postManage;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<PostCreatedEvent>(message);

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
