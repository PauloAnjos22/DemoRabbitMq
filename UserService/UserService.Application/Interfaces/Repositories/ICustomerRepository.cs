using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> FindCustomerAsync(Guid customer);
    }
}
