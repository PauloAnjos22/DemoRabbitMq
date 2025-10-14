using UserService.Application.DTOs;
using UserService.Domain.Events;

namespace UserService.Application.Interfaces.Messaging
{
    public interface IMessagePublisher
    {
        Task<bool> PublishAsync(CustomerPaymentEvent request); // send events to RabbitMQ (but what this method receive on his parameters?)
    }
}
