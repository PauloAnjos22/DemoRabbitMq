using UserService.Application.DTOs.Common;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;

namespace UserService.Application.Services
{
    public class CustomerAccountValidator : ICustomerAccountValidator
    {
        private readonly IBankAccountRepository _bankAccountRepository;

        public CustomerAccountValidator(IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<ResultResponse> ValidateAsync(Guid customer)
        {
            var customerAccount = await _bankAccountRepository.FindCustomerBankAccountAsync(customer);
            if (customerAccount == null)
                return ResultResponse.Fail("Customer account not found");

            return ResultResponse.Ok();
        }
    }
}
