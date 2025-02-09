using RabbitMQ.Client;

namespace Minimart_Api.Services.RabbitMQ
{
    public interface IRabbitMqConnection
    {
        IConnection connection { get; }
    }
}
