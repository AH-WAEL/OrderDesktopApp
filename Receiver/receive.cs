using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using reciever.Hubs;
using reciever;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace reciever
{
    internal class Receive
    {
        private readonly IHubContext<reciever.Hubs.Hubs> _hubContext;

        public Receive(IHubContext<reciever.Hubs.Hubs> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task StartReceiving()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);
            await channel.QueueDeclareAsync(queue: "orderFeed", durable: false, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueBindAsync(queue: "orderFeed", exchange: "logs", routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _hubContext.Clients.All.SendAsync("ReceiveOrder" , message);
                Console.WriteLine($" [x] Received {message}");
                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queue: "orderFeed", autoAck: true, consumer: consumer);
            Console.WriteLine("Waiting for messages. Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
