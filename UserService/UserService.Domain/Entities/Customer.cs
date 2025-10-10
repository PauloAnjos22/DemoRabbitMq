namespace UserService.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        required
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
