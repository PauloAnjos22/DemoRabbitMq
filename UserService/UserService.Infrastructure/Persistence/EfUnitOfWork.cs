using Microsoft.EntityFrameworkCore.Storage;
using UserService.Application.Interfaces.Repositories;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories
{
    public class EfUnitOfWork : IEfUnitOfWork
    {
        private readonly AppServiceDbContext _context;
        private IDbContextTransaction? _transaction; 

        public EfUnitOfWork(AppServiceDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _transaction!.CommitAsync(); 
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction!.RollbackAsync();
        }
    }
}
