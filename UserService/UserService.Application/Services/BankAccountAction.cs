using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;

namespace UserService.Application.Services
{
    public class BankAccountAction : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;

        public BankAccountAction(IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<bool> TransferAsync(Guid fromCustomerId, Guid toCustomerId, long amount)
        {
            return await _bankAccountRepository.DebitCustomerAccountAsync(fromCustomerId, toCustomerId, amount);
        }
    }
}
