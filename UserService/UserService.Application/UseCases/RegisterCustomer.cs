using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Customer;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.UseCases;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases
{
    public class RegisterCustomer : IRegisterCustomer
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public RegisterCustomer(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<ResultResponse> RegisterCustomerAsync(CreateCustomerDto request) 
        {
            var verifyCustomer = await _customerRepository.ExistsByEmailAsync(request.Email);
            if (verifyCustomer)
            {
                return ResultResponse.Fail("Já existe um usuário com o Email fornecido");
            }

            var customerMapped = _mapper.Map<Customer>(request);
            var customer = await _customerRepository.AddAsync(customerMapped);
            if (!customer)
            {
                return ResultResponse.Fail("Falha ao registrar cliente");
            }

            return ResultResponse.Ok();
        }
    }
}
