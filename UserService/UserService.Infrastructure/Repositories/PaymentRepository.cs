using UserService.Application.Interfaces.Repositories;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppServiceDbContext _context;

        public PaymentRepository(AppServiceDbContext context)
        {
            _context = context;
        }
        public async Task<bool> SaveAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
