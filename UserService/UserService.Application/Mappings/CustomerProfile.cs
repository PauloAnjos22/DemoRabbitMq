using AutoMapper;
using UserService.Application.DTOs.Customer;
using UserService.Domain.Entities;

namespace UserService.Application.Mappings
{
    public class CustomerProfile : Profile 
    {
        public CustomerProfile() {
            CreateMap<Customer, ResponseCustomerDto>().ReverseMap();
            CreateMap<Customer, CreateCustomerDto>().ReverseMap();
        }    
    }
}
