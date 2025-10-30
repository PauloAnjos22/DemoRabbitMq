using UserService.Application.DTOs.Common;

namespace UserService.Application.Interfaces.UseCases
{
    public interface IDepositFunds
    {
        Task<ResultResponse> DepositFundsAsync(Guid customer, long amount);
    }
}
