namespace UserService.Domain.Entities
{
    public class BankAccount
    {
        public Guid Id { get; set; }
        public Guid Ownership { get; set; }
        public int MoneyAmount { get; set; }
        public string? AccountType { get; set; }

    }
}
