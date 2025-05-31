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

            var uri = Environment.GetEnvironmentVariable("RABBITMQ_URI"); // store it in Render env


            var factory = new ConnectionFactory
            {
                //HostName = "localhost",
                //UserName = "guest",
                //Password = "guest",

                Uri = new Uri(uri)
            };


            //var factory = new ConnectionFactory
            //{
            //    HostName = "172.31.90.20", // 👈 EC2's private IP
            //    UserName = "guest",
            //    Password = "guest",
            //    Port = 5672,
            //    VirtualHost = "/"
            //};

            //var factory = new ConnectionFactory
            //{
            //    Uri = new Uri(Environment.GetEnvironmentVariable("RABBITMQ_URI"))
            //};

            //var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

            _connection = await factory.CreateConnectionAsync();
        }

        public void Dispose() { 
            _connection?.Dispose();
        }
    }
}
