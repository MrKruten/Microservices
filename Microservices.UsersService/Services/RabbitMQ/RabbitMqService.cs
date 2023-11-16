using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Microservices.UsersService.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://admin:admin@messageBroker:5672"),
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "LabaQueue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: "LabaQueue",
                    basicProperties: null,
                    body: body);
            }
        }
    }
}
