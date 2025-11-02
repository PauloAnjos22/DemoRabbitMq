using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Customer;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;

namespace UserService.Application.Services
{
    public class RegisterValidator : IRegisterValidator
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<RegisterValidator> _logger;

        public RegisterValidator(
            ICustomerRepository customerRepository,
            ILogger<RegisterValidator> logger
            )
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<ResultResponse> ValidateAsync(CreateCustomerDto request)
        {
            var emailExists = await _customerRepository.ExistsByEmailAsync(request.Email);
            if (emailExists)
            {
                _logger.LogWarning("Registration validation failed: email already exists. Email={Email}", request.Email);
                return ResultResponse.Fail("Customer email already exists");
            }

            return ResultResponse.Ok();
        }
    }
}
