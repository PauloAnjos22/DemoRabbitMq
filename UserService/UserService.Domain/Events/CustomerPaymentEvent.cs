namespace UserService.Domain.Events
{
    public class UserPaymentEvent
    {
        public Guid TransactionId { get; init; }
        public Guid AccountFrom { get; init; }
        public Guid AccountTo { get; init; }
        public string? PaymentMethod { get; init; } 

        public int PaymentAmount { get; init; }
        public DateTime Created { get; init; }

        public UserPaymentEvent(Guid transactionId, Guid accountFrom, Guid accountTo, string? paymentMethod, int paymentAmount)
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
