using UserService.Application.DTOs;

namespace UserService.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ResultResponse> ProcessPayment(PaymentDto request); // creates payment entity and publishes UserPaymentEvent
    }
}
