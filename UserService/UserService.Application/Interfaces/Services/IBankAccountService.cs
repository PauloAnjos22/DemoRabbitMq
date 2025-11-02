namespace UserService.Application.Interfaces.Services
{
    public interface IBankAccountService
    {
        Task<bool> TransferAsync(Guid fromCustomerId, Guid toCustomerId, long amount);
    }
}
