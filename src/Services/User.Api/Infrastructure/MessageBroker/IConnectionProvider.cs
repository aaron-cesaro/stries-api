using RabbitMQ.Client;
using System;

namespace User.Api.Infrastructure.MessageBroker
{
    public interface IConnectionProvider : IDisposable
    {
        IConnection GetConnection();
    }
}
