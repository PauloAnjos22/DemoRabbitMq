using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Common;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Interfaces.UseCases;

namespace UserService.Application.UseCases
{
    public class DepositFunds : IDepositFunds
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IEfUnitOfWork _efUnitOfWork;
        private readonly ILogger<DepositFunds> _logger;
        private readonly ICustomerAccountValidator _customerAccountValidator;

        public DepositFunds(
            IBankAccountRepository bankAccountRepository,
            IEfUnitOfWork efUnitOfWork,
            ILogger<DepositFunds> logger,
            ICustomerAccountValidator customerAccountValidator
            )
        {
            _bankAccountRepository = bankAccountRepository;
            _efUnitOfWork = efUnitOfWork;
            _logger = logger;
            _customerAccountValidator = customerAccountValidator;
        }

        public async Task<ResultResponse> DepositFundsAsync(Guid customer, long amount)
        {
            var validator = await _customerAccountValidator.ValidateAsync( customer );
            if (!validator.Success)
            {
                return ResultResponse.Fail(validator.ErrorMessage ?? "Customer account not found");
            }
            await _efUnitOfWork.BeginTransactionAsync();

            try
            {
                var deposit = await _bankAccountRepository.DepositFundsAsync(customer, amount);
                if (!deposit)
                {
                    await _efUnitOfWork.RollbackTransactionAsync();
                    return ResultResponse.Fail("Deposit failed");
                }

                await _efUnitOfWork.CommitTransactionAsync();
                return ResultResponse.Ok();
            }
            catch (Exception ex)
            {
                await _efUnitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error processing deposit funds");
                return ResultResponse.Fail("Deposit failed");
            }
        }
    }
}
