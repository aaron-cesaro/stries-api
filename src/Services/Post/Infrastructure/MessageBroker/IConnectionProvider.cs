using RabbitMQ.Client;
using System;

namespace Post.Infrastructure.MessageBroker
{
    public interface IConnectionProvider : IDisposable
    {
        IConnection GetConnection();
    }
}
