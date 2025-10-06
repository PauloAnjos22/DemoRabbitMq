using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerRepository(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer?> FindCustomerAsync(Guid customer)
        {
            var findCustomer = await _context.FindAsync(customer);
            if (findCustomer == null) {
                return null;
            }

            return findCustomer;
        }
}
