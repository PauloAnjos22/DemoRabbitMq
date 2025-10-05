namespace UserService.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ResultResponse> ProcessPayment(PaymentDto request);
    }
}
