using AutoMapper;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases
{
    public class CustomerPayment : ICustomerPaymentUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IPaymentRepository _paymentRepository;

        public CustomerPayment(ICustomerRepository customerRepository, IMapper mapper, IPaymentRepository paymentRepository)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _paymentRepository = paymentRepository;
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

            var customerFrom = await _customerRepository.FindCustomerAsync(request.From);
            if (customerFrom == null)
            {
                return ResultResponse.Fail("Cliente de origem não encontrado");
            }

            var customerTo = await _customerRepository.FindCustomerAsync(request.To);
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
        }
    }
}
