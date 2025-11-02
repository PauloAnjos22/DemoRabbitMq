using AutoMapper;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Payment;
using UserService.Application.Interfaces.Messaging;
using UserService.Application.Interfaces.Repositories;
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


        public PaymentCustomer(
            ICustomerRepository customerRepository,
            IMapper mapper,
            IPaymentRepository paymentRepository,
            IMessagePublisher<CustomerPaymentEvent> paymentPublisher,
            IMessagePublisher<TransactionCompletedEvent> transactionPublisher,
            IBankAccountRepository bankAccountRepository,
            IEfUnitOfWork efUnitOfWork,
            ILogger<PaymentCustomer> logger
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
        }

        public async Task<ResultResponse> ProcessPayment(CreatePaymentRequest request)
        {
            if (request.Amount <= 0)  
            {
                _logger.LogWarning("Payment failed: invalid amount. Amount={Amount}", request.Amount);
                return ResultResponse.Fail("Payment amount must be greater than zero");
            }

            var customerFrom = await _customerRepository.FindByIdAsync(request.From);
            if (customerFrom == null)
            {
                _logger.LogWarning("Payment failed: sender customer not found. CustomerId={CustomerId}", request.From);
                return ResultResponse.Fail("Failed to find sender customer");
            }

            var customerTo = await _customerRepository.FindByIdAsync(request.To);
            if (customerTo == null)
            {
                _logger.LogWarning("Payment failed: receiver customer not found. CustomerId={CustomerId}", request.To);
                return ResultResponse.Fail("Failed to find receiver customer");
            }

            var payerAccount = await _bankAccountRepository.FindCustomerBankAccountAsync(request.From);
            if (payerAccount == null) {
                _logger.LogWarning("Payment failed: sender account not found. CustomerId={CustomerId}", request.From);
                return ResultResponse.Fail("Failed to find sender customer account");
            }
            var receiverAccount = await _bankAccountRepository.FindCustomerBankAccountAsync(request.To);
            if (receiverAccount == null)
            {
                _logger.LogWarning("Payment failed: receiver account not found. CustomerId={CustomerId}", request.To);
                return ResultResponse.Fail("Failed to find receiver customer account");
            }

            if (payerAccount.Balance < request.Amount)
            {
                var failTransactionEvent = new TransactionCompletedEvent(Guid.NewGuid(), request.From, request.To, request.Amount, "BRL", request.Method, "Fail", "Insufficient funds");
                var failtransactionPublished = await _transactionPublisher.PublishAsync(failTransactionEvent);
                _logger.LogWarning("Payment declined: insufficient funds. CustomerId={CustomerId}, Balance={Balance}, RequestedAmount={Amount}",
                request.From, payerAccount.Balance, request.Amount);
                return ResultResponse.Fail("Insufficient funds to complete the transaction");
            }

            await _efUnitOfWork.BeginTransactionAsync();

            try
            {
                payerAccount.Balance -= request.Amount;
                receiverAccount.Balance += request.Amount;

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
