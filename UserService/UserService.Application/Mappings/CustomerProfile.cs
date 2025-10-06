using AutoMapper;
using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Mappings
{
    public class CustomerProfile : Profile 
    {
        public CustomerProfile() {
            CreateMap<Customer, CustomerDto>().ReverseMap();
        }    
    }
}
