namespace UserService.Application.DTOs.Payment
{
    public class CreatePaymentDto
    {
        public Guid From { get; set; }
        public Guid To { get; set; }
        public int Amount { get; set; }
        public string? Method { get; set; }
    }
}
