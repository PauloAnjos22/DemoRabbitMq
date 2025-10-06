using AutoMapper;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.UseCases
{
    public class Payment : IPaymentUseCase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public Payment(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<ResultResponse> ProcessPayment(PaymentDto request)
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


        }
    }
}
