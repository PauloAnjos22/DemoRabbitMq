using UserService.Application.DTOs;

namespace UserService.Application.Interfaces
{
    public interface IMessagePublisher
    {
        Task<ResultResponse> PublishAsync(); // send events to RabbitMQ (but what this method receive on his parameters?)
    }
}
