using UserService.Domain.Entities;
using UserService.Application.Interfaces.Repositories;
using UserService.Infrastructure.Persistence;
using UserService.Application.DTOs.Customer;
using Microsoft.EntityFrameworkCore;

namespace UserService.Infrastructure.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly AppServiceDbContext _context;

        public BankAccountRepository(AppServiceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> OpenBankAccountAsync(Guid customer)
        {
            if (customer == Guid.Empty)
                return false;

            var bankAccount = new BankAccount();
            bankAccount.CustomerId = customer;
            bankAccount.Balance = 0;

            await _context.BankAccounts.AddAsync(bankAccount);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<BankAccount?> FindCustomerBankAccountAsync(Guid customer)
        {
            if (customer == Guid.Empty)
                return null;

            return await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.CustomerId == customer);
        }

        public async Task<IEnumerable<BankAccount?>> GetAllCustomersBankAccountsAsync()
        {
            return await _context.BankAccounts.ToListAsync();
        }

        public async Task<IEnumerable<(BankAccount Account, Customer Customer)>> GetAllAccountsWithCustomersAsync()
        {
            var result = await _context.BankAccounts
                .Join(_context.Customers,
                    account => account.CustomerId,
                    customer => customer.Id,
                    (account, customer) => new { Account = account, Customer = customer })
                .ToListAsync();

            return result.Select(x => (x.Account, x.Customer));
        }

        public async Task<bool> DebitCustomerAccountAsync(Guid fromCustomerId, Guid toCustomerId, long amount)
        {
            var payerAccount = await _context.BankAccounts.FirstOrDefaultAsync(c => c.CustomerId == fromCustomerId);
            var receiverAccount = await _context.BankAccounts.FirstOrDefaultAsync(c => c.CustomerId == toCustomerId);

            if(payerAccount == null || receiverAccount == null)
                return false;

            payerAccount.Balance -= amount;
            receiverAccount.Balance += amount;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DepositFundsAsync(Guid Customer, long amount)
        {
            var customerAccount = await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.CustomerId == Customer);
            if(customerAccount == null) return false;

            customerAccount.Balance += amount;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
