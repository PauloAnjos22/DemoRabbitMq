using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Services
{
    public interface IPaymentEventPublisherService
    {
        Task PublishPaymentCompletedAsync(Payment payment);
        Task PublishPaymentFailedAsync(Guid from, Guid to, long amount, string? method, string reason);
    }
}
