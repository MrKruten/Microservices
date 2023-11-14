using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microservices.LabService.Services
{
    public class ConsumerService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://admin:admin@messageBroker:5672"),
            };
            await Task.Delay(5000);
            await TryConnectToRabbitMq(factory);
            await Task.CompletedTask;
        }

        private async Task TryConnectToRabbitMq(ConnectionFactory factory)
        {
            try
            {
                ConnectToRabbitMq(factory);
            }
            catch
            {
                Console.WriteLine("rabbitmq: error connect");
                await Task.Delay(5000);
                await TryConnectToRabbitMq(factory);
            }
        }

        private void ConnectToRabbitMq(ConnectionFactory factory)
        {
            Console.WriteLine("rabbitmq: try connect");
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "LabaQueue",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($" {DateTime.Now}::: Received {message}");
                    };
                    channel.BasicConsume(queue: "LabaQueue",
                        autoAck: true,
                        consumer: consumer);

                    while (true){}
                }
            }
        }
    }
}
