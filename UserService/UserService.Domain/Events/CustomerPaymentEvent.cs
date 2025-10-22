namespace UserService.Domain.Events
{
    public class CustomerPaymentEvent
    {
        public Guid TransactionId { get; init; }
        public Guid AccountFrom { get; init; }
        public Guid AccountTo { get; init; }
        public string? PaymentMethod { get; init; } 

        public long PaymentAmount { get; init; }
        public DateTime Created { get; init; }

        public CustomerPaymentEvent(Guid transactionId, Guid accountFrom, Guid accountTo, string? paymentMethod, long paymentAmount)
        {
            TransactionId = transactionId;
            AccountFrom = accountFrom;
            AccountTo = accountTo;
            PaymentMethod = paymentMethod;
            PaymentAmount = paymentAmount;
            Created = DateTime.Now;
        }
    }
}
