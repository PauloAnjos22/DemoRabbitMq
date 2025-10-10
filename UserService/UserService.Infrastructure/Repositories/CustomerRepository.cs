using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppServiceDbContext _context;

        public CustomerRepository(AppServiceDbContext context)
        {
            _context = context;
        }
        public async Task<Customer?> FindCustomerAsync(Guid customer)
        {
            var findCustomer = await _context.Customers.FindAsync(customer);
            if (findCustomer == null)
            {
                return null;
            }

            return findCustomer;
        }
    }
}
