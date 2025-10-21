using UserService.Application.Interfaces.Repositories;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories
{
    public class TransactionLogRepository : ITransactionLogRepository
    {
        private readonly AppServiceDbContext _context;

        public TransactionLogRepository(AppServiceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> saveAsync(TransactionLog log)
        {
            await _context.TransactionLogs.AddAsync(log);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
