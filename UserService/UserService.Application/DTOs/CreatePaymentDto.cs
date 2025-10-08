namespace UserService.Application.DTOs
{
    public class PaymentDto
    {
        public Guid From { get; set; }
        public Guid To { get; set; }
        public int Amount { get; set; }
        public string? Method { get; set; }
    }
}
