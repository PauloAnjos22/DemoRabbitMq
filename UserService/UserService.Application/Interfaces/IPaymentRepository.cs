using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<bool> SaveAsync(Payment payment);
    }
}
