using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<bool> SaveAsync(Payment payment);
    }
}
