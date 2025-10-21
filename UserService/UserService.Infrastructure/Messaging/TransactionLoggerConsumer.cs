using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Entities;
using UserService.Domain.Events;

namespace UserService.Infrastructure.Messaging
{
    public class TransactionLoggerConsumer : BackgroundService
    {
        private readonly ILogger<TransactionLoggerConsumer> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private const string ExchangeName = "transactioncompletedevent";
        private const string QueueName = "payment-logs";

        private static readonly ConcurrentBag<TransactionCompletedEvent> _logs = new();

        public TransactionLoggerConsumer(ILogger<TransactionLoggerConsumer> logger, IServiceScopeFactory scopeFactory)
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

            _logger.LogInformation("TransactionLoggerConsumer connected and bound to exchange {Exchange} queue {Queue}", ExchangeName, QueueName);

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                _logger.LogError("Channel is not initialized");
                return;
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    _logger.LogInformation("TransactionLogger received: {Message}", message);

                    var transactionEvent = JsonSerializer.Deserialize<TransactionCompletedEvent>(message);
                    if (transactionEvent != null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var loggerService = scope.ServiceProvider.GetRequiredService<ITransactionLoggerService>();
                        var logEntry = new TransactionLog
                        {
                            TransactionId = transactionEvent.TransactionId,
                            AccountFrom = transactionEvent.AccountFrom,
                            AccountTo = transactionEvent.AccountTo,
                            Amount = transactionEvent.PaymentAmount,
                            Currency = transactionEvent.Currency,
                            PaymentMethod = transactionEvent.PaymentMethod,
                            Status = transactionEvent.Status ?? "Completed",
                            LoggedAt = DateTime.UtcNow
                        };
                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        _logger.LogWarning("TransactionLogger received non-payment message");
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error logging transaction");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
                }

                await Task.Yield();
            };

            await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);

            // Keep running until cancellation
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TransactionLoggerConsumer stopping");

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
