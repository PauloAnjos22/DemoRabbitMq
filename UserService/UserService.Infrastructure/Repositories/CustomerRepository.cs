using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces.Repositories;
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
        public async Task<Customer?> FindByIdAsync(Guid customer)
        {
            var findCustomer = await _context.Customers.FindAsync(customer);
            if (findCustomer == null)
            {
                return null;
            }

            return findCustomer;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var findCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (findCustomer == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var customers = await _context.Customers.ToListAsync();
            return customers;
        }
    }
}
