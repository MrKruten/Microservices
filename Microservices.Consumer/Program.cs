using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Microservices.Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://admin:admin@messageBroker:5672"),
            };
            Thread.Sleep(5000);
            TryConnectToRabbitMq(factory);
        }

        static private void TryConnectToRabbitMq(ConnectionFactory factory)
        {
            try
            {
                ConnectToRabbitMq(factory);
            }
            catch
            {
                Console.WriteLine("rabbitmq: error connect");
                Thread.Sleep(5000);
                TryConnectToRabbitMq(factory);
            }
        }

        static private void ConnectToRabbitMq(ConnectionFactory factory)
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

                    Console.WriteLine("Type 'exit' to exit.");
                    while (Console.ReadLine() != "exit")
                    {
                    }
                }
            }
        }
    }
}