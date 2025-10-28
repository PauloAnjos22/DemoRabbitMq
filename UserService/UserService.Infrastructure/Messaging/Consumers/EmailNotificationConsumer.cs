using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Events;
namespace UserService.Infrastructure.Messaging.Consumers
{
    public class EmailNotificationConsumer : BackgroundService
    {
        private readonly ILogger<EmailNotificationConsumer> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private const string ExchangeName = "customerpaymentevent";
        private const string QueueName = "payment-emails";

        public EmailNotificationConsumer(ILogger<EmailNotificationConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory; 
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            // Connect to RabbitMQ when service starts
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);
            await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: string.Empty);

            _logger.LogInformation("EmailNotificationConsumer connected and bound to exchange {Exchange} queue {Queue}", ExchangeName, QueueName);

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                _logger.LogError("Channel is not initialized");
                return;
            }

            _logger.LogInformation("Waiting 15 seconds before starting consumer...");
            await Task.Delay(15000, stoppingToken); 

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Received payment event: {Message}", message);
                    await Task.Delay(10000); // 10000 = 10 segundos

                    // Deserialize the payment event
                    var paymentEvent = JsonSerializer.Deserialize<CustomerPaymentEvent>(message);

                    if (paymentEvent != null)
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                            var result = await emailService.PaymentConfirmationAsync(paymentEvent);

                            if (result.Success)
                            {
                                _logger.LogInformation("Email sent successfully for payment {TransactionId}",
                                    paymentEvent.TransactionId);

                                // Acknowledge message (remove from queue)
                                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                            }
                            else
                            {
                                _logger.LogWarning("Failed to send email: {Error}", result.ErrorMessage);

                                // Reject and requeue message
                                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing payment event");
                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false,
                consumer: consumer);

            // Keep running until cancellation
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("EmailNotificationConsumer stopping");

            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
