namespace UserService.Domain.Entities
{
    public class BankAccount
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public long MoneyAmount { get; set; }
        public string? AccountType { get; set; }

    }
}
