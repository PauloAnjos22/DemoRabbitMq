using UserService.Application.Interfaces.Messaging;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Entities;
using UserService.Domain.Events;

namespace UserService.Application.Services
{
    public class PaymentEventPublisher : IPaymentEventPublisherService
    {
        private readonly IMessagePublisher<CustomerPaymentEvent> _paymentPublisher;
        private readonly IMessagePublisher<TransactionCompletedEvent> _transactionPublisher;

        public PaymentEventPublisher(
            IMessagePublisher<CustomerPaymentEvent> paymentPublisher, 
            IMessagePublisher<TransactionCompletedEvent> transactionPublisher
            )
        {
            _paymentPublisher = paymentPublisher;
            _transactionPublisher = transactionPublisher;
        }
        public async Task PublishPaymentCompletedAsync(Payment payment)
        {
            await _paymentPublisher.PublishAsync(
                new CustomerPaymentEvent(payment.Id, payment.From, payment.To, payment.Method, payment.Amount)
            );

            await _transactionPublisher.PublishAsync(
                new TransactionCompletedEvent(payment.Id, payment.From, payment.To, payment.Amount,
                "BRL", payment.Method, "Completed", null)
            );
        }

        public async Task PublishPaymentFailedAsync(Guid from, Guid to, long amount, string? method, string reason)
        {
            await _transactionPublisher.PublishAsync(
                new TransactionCompletedEvent(Guid.NewGuid(), from, to, amount, "BRL", method, "Failed", reason)
            ); 
        }
    }
}
