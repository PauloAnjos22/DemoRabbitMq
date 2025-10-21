using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UserService.Application.Interfaces.Messaging;
using UserService.Domain.Events;

namespace UserService.Infrastructure.Messaging
{
    public class RabbitMQMessagePublisher : IMessagePublisher
    {
        private const string ExchangeName = "payment-exchange";
        public async Task<bool> PublishAsync(CustomerPaymentEvent request)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "rabbitmq",
                    UserName = "guest",
                    Password = "guest"
                };

                // Cria conexão assíncrona
                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                // Declare exchange
                await channel.ExchangeDeclareAsync(exchange: ExchangeName,
                                                  type: ExchangeType.Fanout,
                                                  durable: true,
                                                  autoDelete: false,
                                                  arguments: null);

                var jsonMessage = JsonSerializer.Serialize(request);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                var properties = new BasicProperties();
                properties.Persistent = false;

                // Publish
                await channel.BasicPublishAsync(exchange: ExchangeName,
                                                routingKey: string.Empty,
                                                mandatory: false,
                                                basicProperties: properties,
                                                body: body);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
