using UserService.Domain.Entities;
using UserService.Application.Interfaces.Repositories;
using UserService.Infrastructure.Persistence;

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
            bankAccount.MoneyAmount = 0;

            await _context.BankAccounts.AddAsync(bankAccount);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
