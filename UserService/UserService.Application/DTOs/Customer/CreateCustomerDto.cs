namespace UserService.Application.DTOs.Customer
{
    public class CreateCustomerDto
    {
        public string? Name { get; set; }
        required
        public string Email { get; set; } 
        public string? PhoneNumber { get; set; } 
    }
}
