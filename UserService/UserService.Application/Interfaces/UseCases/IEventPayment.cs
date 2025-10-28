using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Payment;

namespace UserService.Application.Interfaces.UseCases
{
    public interface IEventPayment
    {
        Task<ResultResponse> ProcessPayment(CreatePaymentRequest request); // creates payment entity and publishes UserPaymentEvent
    }
}
