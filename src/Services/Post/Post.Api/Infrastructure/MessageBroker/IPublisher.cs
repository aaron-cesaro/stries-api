using System;
using System.Collections.Generic;

namespace Post.Api.Infrastructure.MessageBroker
{
    public interface IPublisher : IDisposable
    {
        void Publish(string message, IDictionary<string, object> messageAttributes, string timeToLive = null);
    }
}
