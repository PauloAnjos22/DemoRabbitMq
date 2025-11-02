using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Payment;

namespace UserService.Application.Interfaces.Services
{
    public interface IPaymentValidatorService
    {
        Task<ResultResponse> ValidateAsync(CreatePaymentRequest request);
    }
}
