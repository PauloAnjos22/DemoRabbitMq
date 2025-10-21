using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
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

        public EmailService(ILogger<EmailService> logger, ICustomerRepository repository, IOptions<MailSettings> mailSettings)
        {
            _logger = logger;
            _repository = repository;
            _mailSettings = mailSettings.Value;
        }

        public async Task<ResultResponse> PaymentConfirmationAsync(CustomerPaymentEvent paymentEvent)
        {
            if (paymentEvent == null) {
                return ResultResponse.Fail("paymentEvent request is incorrect");
            }

            if (string.IsNullOrWhiteSpace(_mailSettings?.From))
            {
                _logger.LogError("MailSettings.From is not configured");
                return ResultResponse.Fail("MailSettings.From is not configured");
            }

            var customerFrom = await _repository.FindByIdAsync(paymentEvent.AccountFrom);
            var customerTo = await _repository.FindByIdAsync(paymentEvent.AccountTo);

            if (customerFrom == null || customerTo == null)
            {
                return ResultResponse.Fail("Customer not found");
            }

            var messageToReceiver = new MimeMessage();
            messageToReceiver.From.Add(new MailboxAddress(_mailSettings.FromName, _mailSettings.From));
            messageToReceiver.To.Add(MailboxAddress.Parse(customerTo.Email));
            messageToReceiver.Subject = "Payment Confirmation";
            messageToReceiver.Body = new TextPart("plain")
            {
                Text = $"Transação recebida de {customerFrom.Name} no valor de R${paymentEvent.PaymentAmount}"
            };

            var messageToSender = new MimeMessage();
            messageToSender.From.Add(new MailboxAddress(_mailSettings.FromName, _mailSettings.From));
            messageToSender.To.Add(MailboxAddress.Parse(customerFrom.Email));
            messageToSender.Subject = "Payment Confirmation";
            messageToSender.Body = new TextPart("plain")
            {
                Text = $"O pagamento no valor de R${paymentEvent.PaymentAmount} foi enviado com sucesso para {customerTo.Name}!"
            };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                if (!string.IsNullOrWhiteSpace(_mailSettings.User))
                {
                    await smtp.AuthenticateAsync(_mailSettings.User, _mailSettings.Pass);
                }
                
                var errorList = new List<string>();

                try
                {
                    await smtp.SendAsync(messageToReceiver);
                    _logger.LogInformation("Email sent to receiver {Email} for payment {TransactionId}", customerTo.Email, paymentEvent.TransactionId);
                }
                catch(Exception ex)
                {
                    _logger.LogError("Failed to send email to receiver {Email}", customerTo.Email);
                    errorList.Add($"Receiver: {ex.Message}");
                }

                try
                {
                    await smtp.SendAsync(messageToSender);
                    _logger.LogInformation("Email sent to sender {Email} for payment {TransactionId}", customerFrom.Email, paymentEvent.TransactionId);
                }catch(Exception ex)
                {
                    _logger.LogError("Failed to send email to sender {Email}", customerFrom.Email);
                    errorList.Add($"Sender: {ex.Message}");
                }
                
                if(errorList.Count == 0) 
                    return ResultResponse.Ok();

                return ResultResponse.Fail($"{errorList}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when sending payment emails for transaction {TransactionId}", paymentEvent.TransactionId);
                return ResultResponse.Fail($"SMTP error: {ex.Message}");
            }
        }

        public async Task<ResultResponse> RegistrationWelcomeAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
