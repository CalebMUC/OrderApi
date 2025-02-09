using RabbitMQ.Client;
namespace Minimart_Api.Services.RabbitMQ
{
    public class RabbitMqConnection : IRabbitMqConnection, IDisposable
    {
        private IConnection? _connection;

        public IConnection connection => _connection!;

        public RabbitMqConnection() {
            InitializeConnection();
        }

        public async void InitializeConnection() { 
            var factory = new ConnectionFactory { 
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            _connection = await factory.CreateConnectionAsync();
        }

        public void Dispose() { 
            _connection?.Dispose();
        }
    }
}
