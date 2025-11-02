using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Payment;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Interfaces.UseCases;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases
{
    public class PaymentCustomer : IEventPayment
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IEfUnitOfWork _efUnitOfWork;
        private readonly ILogger<PaymentCustomer> _logger;
        private readonly IPaymentValidatorService _paymentValidator;
        private readonly IBankAccountService _bankAccountService;
        private readonly IPaymentEventPublisherService _paymentEventPublisher;


        public PaymentCustomer(
            IPaymentRepository paymentRepository,
            IBankAccountRepository bankAccountRepository,
            IEfUnitOfWork efUnitOfWork,
            ILogger<PaymentCustomer> logger,
            IPaymentValidatorService paymentValidator,
            IBankAccountService bankAccountService,
            IPaymentEventPublisherService paymentEventPublisher
            )
        {
            _paymentRepository = paymentRepository;
            _efUnitOfWork = efUnitOfWork ?? throw new ArgumentNullException(nameof(efUnitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _paymentValidator = paymentValidator;
            _bankAccountService = bankAccountService;
            _paymentEventPublisher = paymentEventPublisher;
        }

        public async Task<ResultResponse> ProcessPayment(CreatePaymentRequest request)
        {
            var validationResult = await _paymentValidator.ValidateAsync(request );
            if (!validationResult.Success)
            {
                await _paymentEventPublisher.PublishPaymentFailedAsync(request.From, request.To, request.Amount, request.Method, "Insufficient funds or account missing"); // Services/PaymentEventPublisher
                return ResultResponse.Fail(validationResult.ErrorMessage ?? "Validation result failed");
            }

            await _efUnitOfWork.BeginTransactionAsync();

            try
            {
                var transferOk = await _bankAccountService.TransferAsync(request.From, request.To, request.Amount);
                if (!transferOk)
                {
                    await _paymentEventPublisher.PublishPaymentFailedAsync(request.From, request.To, request.Amount, request.Method, "Insufficient funds or account missing"); // Services/PaymentEventPublisher
                    await _efUnitOfWork.RollbackTransactionAsync();
                    return ResultResponse.Fail("Transaction failed");
                }

                var payment = new Payment();
                payment.Id = Guid.NewGuid();
                payment.From = request.From;
                payment.To = request.To;
                payment.Amount = request.Amount;
                payment.Method = request.Method;
                payment.CreatedAt = DateTime.Now;

                await _paymentRepository.SaveAsync(payment);
                await _efUnitOfWork.CommitTransactionAsync();

                await _paymentEventPublisher.PublishPaymentCompletedAsync(payment); // Services/PaymentEventPublisher

                return ResultResponse.Ok();
            }
            catch (Exception ex)
            {
                await _efUnitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error processing payment");
                return ResultResponse.Fail("Transaction failed");
            }
        }
    }
}
