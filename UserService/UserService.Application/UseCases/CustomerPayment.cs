using AutoMapper;
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
        private readonly IMessagePublisher _messagePublisher;

        public CustomerPayment(ICustomerRepository customerRepository, IMapper mapper, IPaymentRepository paymentRepository, IMessagePublisher messagePublisher)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _paymentRepository = paymentRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<ResultResponse> ProcessPayment(CreatePaymentDto request)
        {
            if(request == null)
            {
                return ResultResponse.Fail("Invalid request");
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

            var payment = new Payment();
            payment.Id = Guid.NewGuid();
            payment.From = request.From;
            payment.To = request.To;
            payment.Amount = request.Amount;
            payment.Method = request.Method;
            payment.CreatedAt = DateTime.Now;

            var insertPayment = await _paymentRepository.SaveAsync(payment);
            if (!insertPayment)
            {
                return ResultResponse.Fail("Falha ao salvar a transação");
            }

            var paymentEvent = new UserPaymentEvent(payment.Id, payment.From, payment.To, payment.Method, payment.Amount);
            var publishResult = await _messagePublisher.PublishAsync(paymentEvent);

            if (!publishResult)
            {
                return ResultResponse.Fail("Erro ao publicar mensagem");
            }

            return ResultResponse.Ok();
        }
    }
}
