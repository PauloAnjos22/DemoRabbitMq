using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories
{
    public interface IBankAccountRepository
    {
        Task<bool> OpenBankAccountAsync(Guid customer);
        Task<BankAccount?> FindCustomerBankAccountAsync(Guid customer);
        Task<IEnumerable<BankAccount?>> GetAllCustomersBankAccountsAsync();
        Task<IEnumerable<(BankAccount Account, Customer Customer)>> GetAllAccountsWithCustomersAsync();
        Task<bool> DebitCustomerAccountAsync(Guid fromCustomerId, Guid toCustomerId, long amount); // Update Balance 
        Task<bool> DepositFundsAsync(Guid Customer, long amount); 
    }
}
