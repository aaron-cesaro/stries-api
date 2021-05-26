using System;
using System.Collections.Generic;

namespace User.Api.Infrastructure.MessageBroker
{
    public interface IPublisher : IDisposable
    {
        void Publish(string message, string routingKey, IDictionary<string, object> messageAttributes, string timeToLive = null);
    }
}
