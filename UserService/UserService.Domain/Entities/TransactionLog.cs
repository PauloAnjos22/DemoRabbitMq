namespace UserService.Domain.Entities
{
    public class TransactionLog
    {
        public Guid Id { get; set; } 
        public Guid TransactionId { get; set; }
        public Guid AccountFrom { get; set; }
        public Guid AccountTo { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; }
        public string? FailureReason { get; set; }
        public DateTime LoggedAt { get; set; }
    }
}
