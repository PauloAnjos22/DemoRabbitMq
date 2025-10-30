using UserService.Application.DTOs.Common;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.UseCases;

namespace UserService.Application.UseCases
{
    public class DepositFunds : IDepositFunds
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IEfUnitOfWork _efUnitOfWork;

        public DepositFunds(
            IBankAccountRepository bankAccountRepository, 
            IEfUnitOfWork efUnitOfWork
            )
        {
            _bankAccountRepository = bankAccountRepository;
            _efUnitOfWork = efUnitOfWork;
        }

        public async Task<ResultResponse> DepositFundsAsync(Guid customer, long amount)
        {
            var findBankAccount = await _bankAccountRepository.FindCustomerBankAccountAsync( customer );
            if (findBankAccount == null)
            {
                return ResultResponse.Fail("Customer Bank Account not found");
            }

            if (amount <= 0)
            {
                return ResultResponse.Fail("Invalid amount value");
            }

            await _efUnitOfWork.BeginTransactionAsync();
            try
            {

            }
        }
    }
}
