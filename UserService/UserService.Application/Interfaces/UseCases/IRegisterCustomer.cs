using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Customer;

namespace UserService.Application.Interfaces.UseCases
{
    public interface IRegisterCustomer
    {
        Task<ResultResponse> RegisterCustomerAsync(CreateCustomerDto request);
    }
}
