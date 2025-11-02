using AutoMapper;
using UserService.Application.DTOs.Customer;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.UseCases;

namespace UserService.Application.UseCases
{
    public class GetAllCustomersAccount : IGetCustomersAccount
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IMapper _mapper;

        public GetAllCustomersAccount(
            IBankAccountRepository bankAccountRepository,
            IMapper mapper
            )
        {
            _bankAccountRepository = bankAccountRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerAccountDto?>> GetCustomersAccountAsync()
        {
            var accountsWithCustomers = await _bankAccountRepository.GetAllAccountsWithCustomersAsync();

            var result = _mapper.Map<IEnumerable<CustomerAccountDto>>(accountsWithCustomers);

            return result;
        }
    }
}
