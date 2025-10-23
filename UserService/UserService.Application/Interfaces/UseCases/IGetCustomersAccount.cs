using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Customer;

namespace UserService.Application.Interfaces.UseCases
{
    public interface IGetCustomersAccount
    {
        Task<IEnumerable<CustomerAccountDto?>> GetCustomersAccountAsync();
    }
}
