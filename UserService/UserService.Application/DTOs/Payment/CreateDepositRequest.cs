namespace UserService.Application.DTOs.Payment
{
    public class CreateDepositRequest
    {
        public Guid CustomerId { get; set; }
        public long Amount { get; set; }
    }
}
