using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence
{
    public class AppServiceDbContext : DbContext 
    {
        public AppServiceDbContext(DbContextOptions<AppServiceDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<BankAccount> BankAccounts { get; set; } = null!;
        public DbSet<AuditLog> TransactionLogs { get; set; } = null!;
    }
}
