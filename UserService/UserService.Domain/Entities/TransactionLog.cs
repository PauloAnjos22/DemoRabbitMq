namespace UserService.Domain.Entities
{
    public class TransactionLog
    {
        public Guid Id { get; set; } // chave primária da tabela
        public Guid TransactionId { get; set; }
        public Guid AccountFrom { get; set; }
        public Guid AccountTo { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime LoggedAt { get; set; }
    }
}
