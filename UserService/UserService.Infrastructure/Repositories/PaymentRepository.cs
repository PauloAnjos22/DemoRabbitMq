using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public async Task<bool> SaveAsync(Payment payment)
        {
            _context.Payment.Add(payment);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
