using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Customer;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.UseCases
{
    public interface IGetCustomers
    {
        Task<IEnumerable<ResponseCustomerDto>> GetAllAsync();
    }
}
