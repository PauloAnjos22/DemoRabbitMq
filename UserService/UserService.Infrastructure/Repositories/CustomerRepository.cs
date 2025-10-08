using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public async Task<Customer?> FindCustomerAsync(Guid customer)
        {
            var findCustomer = await _context.FindAsync(customer);
            if (findCustomer == null) {
                return null;
            }

            return findCustomer;
        }
}
