using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UserService.Application.Interfaces.Messaging;
using UserService.Domain.Events;

namespace UserService.Infrastructure.Messaging
{
    public class RabbitMQMessagePublisher<T> : IMessagePublisher<T> where T : class // (RESTRIÇÃO para o tipo genérico)
    {
        public async Task<bool> PublishAsync(T request)
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

                var exchangeName = typeof(T).Name.ToLowerInvariant(); // ex: "customerpaymentevent"

                // Declare exchange
                await channel.ExchangeDeclareAsync(exchange: exchangeName,
                                                  type: ExchangeType.Fanout,
                                                  durable: true,
                                                  autoDelete: false,
                                                  arguments: null);

                var jsonMessage = JsonSerializer.Serialize(request);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                var properties = new BasicProperties();
                properties.Persistent = false;

                // Publish
                await channel.BasicPublishAsync(exchange: exchangeName,
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
