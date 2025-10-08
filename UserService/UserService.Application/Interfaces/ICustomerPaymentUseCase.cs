using UserService.Application.DTOs;

namespace UserService.Application.Interfaces
{
    public interface ICustomerPaymentUseCase
    {
        Task<ResultResponse> ProcessPayment(CreatePaymentDto request); // creates payment entity and publishes UserPaymentEvent
    }
}
