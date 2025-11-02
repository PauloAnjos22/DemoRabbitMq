using AutoMapper;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Customer;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Interfaces.UseCases;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases
{
    public class RegisterCustomer : IRegisterCustomer
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IRegisterValidator _registerValidator;
        private readonly IEfUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterCustomer> _logger;

        public RegisterCustomer(
            ICustomerRepository customerRepository, 
            IMapper mapper, 
            IBankAccountRepository bankAccountRepository,
            IRegisterValidator registerValidator,
            IEfUnitOfWork efUnitOfWork,
            ILogger<RegisterCustomer> logger
            )
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _bankAccountRepository = bankAccountRepository;
            _registerValidator = registerValidator;
            _unitOfWork = efUnitOfWork;
            _logger = logger;
        }

        public async Task<ResultResponse> RegisterCustomerAsync(CreateCustomerDto request) 
        {
            var validateRequest = await _registerValidator.ValidateAsync( request );
            if (!validateRequest)
            {
                return ResultResponse.Fail("Customer email already exist");
            }
            var customerMapped = _mapper.Map<Customer>(request);

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var customer = await _customerRepository.AddAsync(customerMapped);
                if (!customer)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError("Failed to add customer. Email={Email}", request.Email);
                    return ResultResponse.Fail("Failed registering customer");
                }

                var customerBankAccount = await _bankAccountRepository.OpenBankAccountAsync(customerMapped.Id);
                if (!customerBankAccount)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError("Failed to open bank account. CustomerId={CustomerId}", customerMapped.Id);
                    return ResultResponse.Fail("Failed to open customer bank account");
                }
                await _unitOfWork.CommitTransactionAsync();
                return ResultResponse.Ok();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error registering customer. Email={Email}", request.Email);
                return ResultResponse.Fail("Error registering customer");
            }
        }
    }
}
