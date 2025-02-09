using Minimart_Api.DTOS;

namespace Minimart_Api.Services.RabbitMQ
{
    public interface IOrderEventPublisher
    {
        public Task PublishOrderEvent(OrderEvent orderEvent);
    }
}
