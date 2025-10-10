using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> FindByIdAsync(Guid customer);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> AddAsync(Customer customer);
    }
}
