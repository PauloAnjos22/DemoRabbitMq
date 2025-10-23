namespace UserService.Application.DTOs.Customer
{
    public class CustomerAccountDto(Guid id, string name, long balance)
    {
        public Guid Id { get; set; } = id;
        public string Name { get; set; } = name;
        public long Balance { get; set; } = balance;
    }
}
