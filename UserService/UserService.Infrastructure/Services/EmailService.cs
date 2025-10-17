using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net.Mail;
using UserService.Application.DTOs.Common;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Entities;
using UserService.Domain.Events;
using UserService.Infrastructure.Configuration;

namespace UserService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly ICustomerRepository _repository;
        private readonly MailSettings _mailSettings;

        public EmailService(ILogger<EmailService> logger, ICustomerRepository repository, MailSettings mailSettings)
        {
            _logger = logger;
            _repository = repository;
            _mailSettings = mailSettings;
        }

        public async Task<ResultResponse> PaymentConfirmationAsync(CustomerPaymentEvent paymentEvent)
        {
            if (paymentEvent == null) {
                return ResultResponse.Fail("paymentEvent request is incorrect");
            }

            var customerFrom = await _repository.FindByIdAsync(paymentEvent.AccountFrom);
            var customerTo = await _repository.FindByIdAsync(paymentEvent.AccountTo);

            if(customerFrom == null || customerTo == null)
            {
                return ResultResponse.Fail("Customer not found");
            }

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.FromName, _mailSettings.From));
            email.To.Add(MailboxAddress.Parse(customerTo.Email));
            email.Subject = "Payment Confirmation";
            email.Body = new TextPart("plain")
            {
                Text = $"{customerFrom.Name} te enviou um pagamento de R${paymentEvent.PaymentAmount}"
            };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.User, _mailSettings.Pass);
                await smtp.SendAsync(email);

                _logger.LogInformation("Email sent successfully to {Email} for payment {TransactionId}",
                    customerTo.Email, paymentEvent.TransactionId);

                return ResultResponse.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", customerTo.Email);
                return ResultResponse.Fail($"Error sending email: {ex.Message}");
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task<ResultResponse> RegistrationWelcomeAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
