namespace UserService.Application.DTOs.Customer
{
    public class CreateCustomerDto
    {
        required
        public string Name { get; set; }
        public string? Email { get; set; } 
        public string? PhoneNumber { get; set; } 
    }
}
