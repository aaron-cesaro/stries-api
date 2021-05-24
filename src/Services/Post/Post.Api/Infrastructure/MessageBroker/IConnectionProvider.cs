using RabbitMQ.Client;
using System;

namespace Post.Api.Infrastructure.MessageBroker
{
    public interface IConnectionProvider : IDisposable
    {
        IConnection GetConnection();
    }
}
