using UserService.Application.DTOs.Common;

namespace UserService.Application.Interfaces.Services
{
    public interface ICustomerAccountValidator
    {
        Task<ResultResponse> ValidateAsync(Guid customer);
    }
}
