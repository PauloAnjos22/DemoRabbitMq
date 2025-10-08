using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain;
using UserService.Domain.Events;

namespace UserService.Infrastructure.Messaging
{
    public class RabbitMQMessagePublisher : IMessagePublisher
    {
        public async Task<bool> PublishAsync(UserPaymentEvent request)
        {
            //RabbitMQMessagePublisher Implementation Steps:
            //1.Add RabbitMQ using statements
            //2.Create connection factory and connection
            //3.Create channel for communication
            //4.Declare queue(create if doesn't exist)
            //5.Serialize UserPaymentEvent to JSON
            //6.Publish message to queue
            //7.Handle errors and return success / failure

            //Key RabbitMQ Classes to Use:
            //1. ConnectionFactory: Creates connections
            //2. IConnection: Represents connection to RabbitMQ
            //3. IModel: Represents channel for communication
            //4. JsonSerializer: Convert event to JSON string
            //5. Encoding.UTF8: Convert string to bytes for RabbitMQ

            //Think About: What do you need to connect to RabbitMQ?
            //- Server address(localhost for learning)
            //- Username/password(guest/guest default)
            //- Queue name(pick something like "payment-events")
            try
            {
                // RabbitMQMessagePublisher Implementation Step 2
                var factory = new ConnectionFactory() // Key RabbitMQ Classes 1

                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"
                };

                // RabbitMQMessagePublisher Implementation Step 3
                await using var connection = await factory.CreateConnectionAsync(); // Key RabbitMQ Classes 2
                using var channel = await connection.CreateChannelAsync();

                // RabbitMQMessagePublisher Implementation Step 4
                string queueName = "payment-events";
                await channel.QueueDeclareAsync(queue: queueName,
                                   durable: true,      
                                   exclusive: false,   
                                   autoDelete: false,  
                                   arguments: null);

                // RabbitMQMessagePublisher Implementation Step 5
                string jsonMessage = JsonSerializer.Serialize(request); // Key RabbitMQ Classes to Use 4
                var body = Encoding.UTF8.GetBytes(jsonMessage); // Key RabbitMQ Classes to Use 5

                var properties = new BasicProperties();
                properties.Persistent = false;

                // RabbitMQMessagePublisher Implementation Step 6
                await channel.BasicPublishAsync(
                    exchange: string.Empty,          
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
