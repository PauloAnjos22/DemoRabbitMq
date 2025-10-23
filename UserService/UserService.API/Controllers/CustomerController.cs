using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Customer;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.UseCases;

namespace UserService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IRegisterCustomer _registerUseCase;
        private readonly IGetCustomers _getCustomers;
        private readonly IGetCustomersAccount _getCustomersAccount;

        public CustomerController(IRegisterCustomer registerUseCase, IGetCustomers getCustomers, IGetCustomersAccount getCustomersAccount)
        {
            _registerUseCase = registerUseCase;
            _getCustomers = getCustomers;
            _getCustomersAccount = getCustomersAccount;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(CreateCustomerDto request)
        {
            var customerResult = await _registerUseCase.RegisterCustomerAsync(request);
            if (!customerResult.Success)
            {
                return BadRequest(customerResult.ErrorMessage);
            }

            return Ok();
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ResponseCustomerDto>>> GetAllCustomers()
        {
            var customers = await _getCustomers.GetAllAsync();
            return Ok(customers);
        }

        [HttpGet("bank-accounts")]
        public async Task<ActionResult<IEnumerable<CustomerAccountDto>>> GetAllCustomersBankAccounts()
        {
            var customerAccounts = await _getCustomersAccount.GetCustomersAccountAsync();

            return Ok(customerAccounts);
        }
    }
}
