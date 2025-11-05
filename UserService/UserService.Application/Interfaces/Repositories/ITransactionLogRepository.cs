using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories
{
    public interface ITransactionLogRepository
    {
        Task<bool> saveAsync(AuditLog log);
    }
}
