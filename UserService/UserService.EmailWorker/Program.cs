using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Infrastructure.Configuration;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;

var builder = Host.CreateApplicationBuilder(args);
// Bind MailSettings for worker
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// DbContext
builder.Services.AddDbContext<AppServiceDbContext>(
    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register repositories required by EmailService
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ITransactionLogRepository, TransactionLogRepository>();

// Register email service and consumers
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<EmailNotificationConsumer>();
builder.Services.AddHostedService<TransactionLoggerConsumer>();

var host = builder.Build();
host.Run();
