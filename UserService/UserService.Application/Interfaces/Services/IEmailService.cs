using UserService.Application.DTOs.Common;
using UserService.Domain.Entities;
using UserService.Domain.Events;

namespace UserService.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<ResultResponse> PaymentConfirmationAsync(CustomerPaymentEvent paymentEvent);
        Task<ResultResponse> RegistrationWelcomeAsync(Customer customer);
    }
}
