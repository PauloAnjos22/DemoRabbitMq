using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Customer;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.UseCases;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases
{
    public class GetAllCustomers : IGetCustomers
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public GetAllCustomers(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResponseCustomerDto>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ResponseCustomerDto>>(customers);
        }
    }
}
