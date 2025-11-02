using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Customer;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;

namespace UserService.Application.Services
{
    public class RegisterValidator : IRegisterValidator
    {
        private readonly ICustomerRepository _customerRepository;

        public RegisterValidator(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<bool> ValidateAsync(CreateCustomerDto request)
        {
            var verifyCustomer = await _customerRepository.ExistsByEmailAsync(request.Email);
            if (!verifyCustomer)
                return false;

            return true;
        }
    }
}
