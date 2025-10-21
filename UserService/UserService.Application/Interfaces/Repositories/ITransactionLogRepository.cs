using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories
{
    public interface ITransitionLogRepository
    {
        Task<bool> saveAsync(TransactionLog log);
    }
}
