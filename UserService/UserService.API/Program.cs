using Microsoft.EntityFrameworkCore;
using UserService.Application.Interfaces.Messaging;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Interfaces.UseCases;
using UserService.Application.Mappings;
using UserService.Application.UseCases;
using UserService.Domain.Entities;
using UserService.Infrastructure.Messaging;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppServiceDbContext>
    (opt =>
        opt.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection") 
        )
    );
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IEventPayment, CustomerPayment>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IMessagePublisher, RabbitMQMessagePublisher>();
builder.Services.AddScoped<IRegisterCustomer, RegisterCustomer>();
builder.Services.AddScoped<IGetCustomers, GetAllCustomers>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CustomerProfile>();
});
// Add RabbitMQ Consumer (Background Service)
builder.Services.AddHostedService<EmailNotificationConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
