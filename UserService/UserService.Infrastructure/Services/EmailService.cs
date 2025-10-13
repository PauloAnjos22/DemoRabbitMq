using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Common;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Entities;
using UserService.Domain.Events;

namespace UserService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task<ResultResponse> PaymentConfirmationAsync(CustomerPaymentEvent paymentEvent)
        {
            if (paymentEvent == null) {
                return ResultResponse.Fail("paymentEvent request is incorrect");
            }
            _logger.LogInformation("Email to sender: {AccountFrom} ", paymentEvent.AccountFrom);
            _logger.LogInformation("You sent a payment of {Amount} to {AccountTo}", paymentEvent.PaymentAmount, paymentEvent.AccountTo);

            _logger.LogInformation("Email to receiver {AccountTo}", paymentEvent.AccountTo);
            _logger.LogInformation("You received a payment of {Amount} by {AccountFrom}", paymentEvent.PaymentAmount, paymentEvent.AccountFrom);

            return ResultResponse.Ok();
        }

        public async Task<ResultResponse> RegistrationWelcomeAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
