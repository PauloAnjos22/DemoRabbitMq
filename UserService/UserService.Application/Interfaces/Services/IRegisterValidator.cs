using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Customer;

namespace UserService.Application.Interfaces.Services
{
    public interface IRegisterValidator
    {
        Task <bool> ValidateAsync(CreateCustomerDto response);
    }
}
