using RabbitMQ.Client;
using System;

namespace User.Infrastructure.MessageBroker
{
    public interface IConnectionProvider : IDisposable
    {
        IConnection GetConnection();
    }
}
