using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Customer;
using UserService.Application.Interfaces.UseCases;

namespace UserService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IRegisterCustomer _registerUseCase;
        private readonly IGetCustomers _getCustomers;

        public CustomerController(IRegisterCustomer registerUseCase, IGetCustomers getCustomers)
        {
            _registerUseCase = registerUseCase;
            _getCustomers = getCustomers;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseCustomerDto>>> GetAllCustomers()
        {
            var customers = await _getCustomers.GetAllAsync();
            return Ok(customers);
        }
    }
}
