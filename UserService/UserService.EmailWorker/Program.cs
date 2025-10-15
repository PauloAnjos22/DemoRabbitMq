using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces.Services;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<AppServiceDbContext>
    (opt =>
        opt.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<EmailNotificationConsumer>();

var host = builder.Build();
host.Run();
