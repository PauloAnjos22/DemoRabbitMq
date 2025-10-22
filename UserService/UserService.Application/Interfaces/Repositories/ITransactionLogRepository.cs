using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories
{
    public interface ITransactionLogRepository
    {
        Task<bool> saveAsync(TransactionLog log);
    }
}
