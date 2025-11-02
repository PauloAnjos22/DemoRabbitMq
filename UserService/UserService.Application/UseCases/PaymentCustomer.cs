using AutoMapper;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Payment;
using UserService.Application.Interfaces.Messaging;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Interfaces.UseCases;
using UserService.Domain.Entities;
using UserService.Domain.Events;

namespace UserService.Application.UseCases
{
    public class PaymentCustomer : IEventPayment
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IMessagePublisher<CustomerPaymentEvent> _paymentPublisher;
        private readonly IMessagePublisher<TransactionCompletedEvent> _transactionPublisher;
        private readonly IEfUnitOfWork _efUnitOfWork;
        private readonly ILogger<PaymentCustomer> _logger;
        private readonly IPaymentValidatorService _paymentValidator;
        private readonly IBankAccountService _bankAccountService;


        public PaymentCustomer(
            ICustomerRepository customerRepository,
            IMapper mapper,
            IPaymentRepository paymentRepository,
            IMessagePublisher<CustomerPaymentEvent> paymentPublisher,
            IMessagePublisher<TransactionCompletedEvent> transactionPublisher,
            IBankAccountRepository bankAccountRepository,
            IEfUnitOfWork efUnitOfWork,
            ILogger<PaymentCustomer> logger,
            IPaymentValidatorService paymentValidator,
            IBankAccountService bankAccountService
            )
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _paymentRepository = paymentRepository;
            _paymentPublisher = paymentPublisher;
            _transactionPublisher = transactionPublisher;
            _bankAccountRepository = bankAccountRepository;
            _efUnitOfWork = efUnitOfWork ?? throw new ArgumentNullException(nameof(efUnitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _paymentValidator = paymentValidator;
            _bankAccountService = bankAccountService;
        }

        public async Task<ResultResponse> ProcessPayment(CreatePaymentRequest request)
        {
            var validationResult = await _paymentValidator.ValidateAsync(request );
            if (!validationResult.Success)
            {
                var failTransactionEvent = new TransactionCompletedEvent(Guid.NewGuid(), request.From, request.To, request.Amount, "BRL", request.Method, "Fail", "Insufficient funds");
                var failtransactionPublished = await _transactionPublisher.PublishAsync(failTransactionEvent);
                return ResultResponse.Fail(validationResult.ErrorMessage ?? "Validation result failed");
            }

            await _efUnitOfWork.BeginTransactionAsync();

            try
            {
                var transferOk = await _bankAccountService.TransferAsync(request.From, request.To, request.Amount);
                if (!transferOk)
                {
                    await _transactionPublisher.PublishAsync(new TransactionCompletedEvent(Guid.NewGuid(), request.From, request.To, request.Amount, "BRL", request.Method, "Fail", "Insufficient funds or account missing"));
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

                var paymentEvent = new CustomerPaymentEvent(payment.Id, payment.From, payment.To, payment.Method, payment.Amount);
                var transactionEvent = new TransactionCompletedEvent(payment.Id, payment.From, payment.To, payment.Amount, "BRL", payment.Method, "Completed", "");

                await _paymentPublisher.PublishAsync(paymentEvent);
                await _transactionPublisher.PublishAsync(transactionEvent);

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
