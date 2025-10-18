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
// MailSettings for the worker
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// DbContext
builder.Services.AddDbContext<AppServiceDbContext>(
    opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register repositories required by EmailService
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Register email service and consumer
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<EmailNotificationConsumer>();

var host = builder.Build();

// Log resolved MailSettings (development-only diagnostic)
try
{
    var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    var mail = host.Services.GetRequiredService<IOptions<MailSettings>>().Value;
    logger.LogInformation("Worker MailSettings loaded: Host={Host} Port={Port} From={From} FromName={FromName}", mail.Host, mail.Port, mail.From, mail.FromName);
}
catch (Exception ex)
{
    Console.WriteLine("Could not read MailSettings: " + ex.Message);
}

host.Run();
