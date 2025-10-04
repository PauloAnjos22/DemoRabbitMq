namespace UserService.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid From { get; set; }
        public Guid To { get; set; }
        public int Amount { get; set; }
        public string? Method { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
