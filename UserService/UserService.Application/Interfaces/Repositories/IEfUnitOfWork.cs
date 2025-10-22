namespace UserService.Application.Interfaces.Repositories
{
    public interface IEfUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
