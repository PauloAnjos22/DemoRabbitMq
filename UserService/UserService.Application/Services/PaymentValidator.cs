
using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Payment;
using UserService.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using UserService.Application.Interfaces.Repositories;

namespace UserService.Application.Services
{
    public class PaymentValidator : IPaymentValidatorService
    {
        private readonly ILogger<PaymentValidator> _logger;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        public PaymentValidator(
            ILogger<PaymentValidator> logger, 
            ICustomerRepository customerRepository,
            IBankAccountRepository bankAccountRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<ResultResponse> ValidateAsync(CreatePaymentRequest request)
        {
            if (request.Amount <= 0)
            {
                _logger.LogWarning("Payment failed: invalid amount. Amount={Amount}", request.Amount);
                return ResultResponse.Fail("Payment amount must be greater than zero");
            }

            var customerFrom = await _customerRepository.FindByIdAsync(request.From);
            if (customerFrom == null)
            {
                _logger.LogWarning("Payment failed: sender customer not found. CustomerId={CustomerId}", request.From);
                return ResultResponse.Fail("Failed to find sender customer");
            }

            var customerTo = await _customerRepository.FindByIdAsync(request.To);
            if (customerTo == null)
            {
                _logger.LogWarning("Payment failed: receiver customer not found. CustomerId={CustomerId}", request.To);
                return ResultResponse.Fail("Failed to find receiver customer");
            }

            var payerAccount = await _bankAccountRepository.FindCustomerBankAccountAsync(request.From);
            if (payerAccount == null)
            {
                _logger.LogWarning("Payment failed: sender account not found. CustomerId={CustomerId}", request.From);
                return ResultResponse.Fail("Failed to find sender customer account");
            }
            var receiverAccount = await _bankAccountRepository.FindCustomerBankAccountAsync(request.To);
            if (receiverAccount == null)
            {
                _logger.LogWarning("Payment failed: receiver account not found. CustomerId={CustomerId}", request.To);
                return ResultResponse.Fail("Failed to find receiver customer account");
            }

            if (payerAccount.Balance < request.Amount)
            {
                _logger.LogWarning("Payment declined: insufficient funds. CustomerId={CustomerId}, Balance={Balance}, RequestedAmount={Amount}",
                request.From, payerAccount.Balance, request.Amount);
                return ResultResponse.Fail("Insufficient funds to complete the transaction");
            }

            return ResultResponse.Ok();
        }
    }
}
