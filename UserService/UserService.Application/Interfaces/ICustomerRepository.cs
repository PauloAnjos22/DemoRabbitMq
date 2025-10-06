using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<Customer?> FindCustomerAsync(Guid customer);
    }
}
