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
    public class CustomerPayment : IEventPayment
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IMessagePublisher<CustomerPaymentEvent> _paymentPublisher;
        private readonly IMessagePublisher<TransactionCompletedEvent> _transactionPublisher;
        private readonly IEfUnitOfWork _efUnitOfWork;
        private readonly ILogger<CustomerPayment> _logger;


        public CustomerPayment(
            ICustomerRepository customerRepository,
            IMapper mapper,
            IPaymentRepository paymentRepository,
            IMessagePublisher<CustomerPaymentEvent> paymentPublisher,
            IMessagePublisher<TransactionCompletedEvent> transactionPublisher,
            IBankAccountRepository bankAccountRepository
            )
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _paymentRepository = paymentRepository;
            _paymentPublisher = paymentPublisher;
            _transactionPublisher = transactionPublisher;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<ResultResponse> ProcessPayment(CreatePaymentDto request)
        {
            if(request == null)
            {
                return ResultResponse.Fail("Dados inválidos");
            }

            if (request.Amount < 0) {
                return ResultResponse.Fail("Valor inválido de transação");
            }

            var customerFrom = await _customerRepository.FindByIdAsync(request.From);
            if (customerFrom == null)
            {
                return ResultResponse.Fail("Cliente de origem não encontrado");
            }

            var customerTo = await _customerRepository.FindByIdAsync(request.To);
            if (customerTo == null)
            {
                return ResultResponse.Fail("Cliente de destino não encontrado");
            }

            var payerAccount = await _bankAccountRepository.FindCustomerBankAccountAsync(request.From);
            if (payerAccount == null) {
                return ResultResponse.Fail("Falha ao encontrar a conta do cliente de origem");
            }
            var receiverAccount = await _bankAccountRepository.FindCustomerBankAccountAsync(request.To);
            if (receiverAccount == null)
            {
                return ResultResponse.Fail("Falha ao encontrar a conta do cliente de destino");
            }

            if (payerAccount.Balance < request.Amount)
            {
                var failTransactionEvent = new TransactionCompletedEvent(Guid.NewGuid(), request.From, request.To, request.Amount, "BRL", request.Method, "Fail", "Insufficient funds");
                var failtransactionPublished = await _transactionPublisher.PublishAsync(failTransactionEvent);
                return ResultResponse.Fail("Saldo insuficiente na conta para realizar a transação");
            }

            using var transaction = await _efUnitOfWork.BeginTransactionAsync();

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
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error processing payment");
                return ResultResponse.Fail("Transaction failed");
            }
        }
    }
}
