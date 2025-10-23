using UserService.Application.DTOs.Customer;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.UseCases;

namespace UserService.Application.UseCases
{
    public class GetAllCustomersAccount : IGetCustomersAccount
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ICustomerRepository _customerRepository;

        public GetAllCustomersAccount(
            IBankAccountRepository bankAccountRepository,
            ICustomerRepository customerRepository)
        {
            _bankAccountRepository = bankAccountRepository;
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerAccountDto?>> GetCustomersAccountAsync()
        {
            var bankAccounts = await _bankAccountRepository.GetAllCustomersBankAccountsAsync() ?? [];
            var result = new List<CustomerAccountDto>();

            foreach(var b in bankAccounts)
            {
                var customer = await _customerRepository.FindByIdAsync(b.CustomerId);
                result.Add(
                    new CustomerAccountDto
                    (
                        b.CustomerId,
                        customer?.Name ?? String.Empty,
                        b.Balance
                    )
                );
            }

            return result;
        }
    }
}
