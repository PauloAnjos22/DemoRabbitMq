using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Payment;

namespace UserService.Application.Interfaces.UseCases
{
    public interface IDepositFunds
    {
        Task<ResultResponse> DepositFundsAsync(CreateDepositRequest request);
    }
}
