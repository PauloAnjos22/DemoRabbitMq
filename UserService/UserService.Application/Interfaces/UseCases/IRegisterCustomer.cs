using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Customer;

namespace UserService.Application.Interfaces.UseCases
{
    public class ICustomerRegister
    {
        Task<ResultResponse> RegisterCustomerAsync(RegisterCustomerDto request);
    }
}
