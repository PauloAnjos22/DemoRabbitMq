using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces.Messaging;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Interfaces.UseCases;
using UserService.Application.Mappings;
using UserService.Application.Services;
using UserService.Application.UseCases;
using UserService.Domain.Entities;
using UserService.Infrastructure.Configuration;
using UserService.Infrastructure.Messaging.Publishers;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppServiceDbContext>
    (opt =>
        opt.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection") 
        )
    );
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IEventPayment, PaymentCustomer>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ITransactionLogRepository, TransactionLogRepository>();
builder.Services.AddScoped<IEfUnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped(typeof(IMessagePublisher<>), typeof(RabbitMQMessagePublisher<>)); // São genéricos agora
builder.Services.AddScoped<IRegisterCustomer, RegisterCustomer>();
builder.Services.AddScoped<IGetCustomers, GetAllCustomers>();
builder.Services.AddScoped<IGetCustomersAccount, GetAllCustomersAccount>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBankAccountService, BankAccountAction>();
builder.Services.AddScoped<IPaymentEventPublisherService, PaymentEventPublisher>();
builder.Services.AddScoped<IPaymentValidatorService, PaymentValidator>();
builder.Services.AddScoped<IRegisterValidator, RegisterValidator>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CustomerProfile>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppServiceDbContext>();
        context.Database.Migrate(); 
        app.Logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating the database");
        throw; 
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
