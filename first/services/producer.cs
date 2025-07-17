using Microsoft.Identity.Client;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace first.services
{
    public class producer
    {
        private ConnectionFactory factory;
        public producer() {
            factory = new ConnectionFactory
            {
                HostName = "host.docker.internal",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
        }

        public async Task send(string message) 
        {
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: "logs", routingKey: "orderFeed", body: body);

            Console.WriteLine($" [x] Sent {message}");

        }

    }
}
