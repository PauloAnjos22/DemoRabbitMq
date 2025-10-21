using UserService.Application.DTOs;
using UserService.Domain.Events;

namespace UserService.Application.Interfaces.Messaging
{
    public interface IMessagePublisher<T>
    {
        Task<bool> PublishAsync(T eventRequest);
    }
}
