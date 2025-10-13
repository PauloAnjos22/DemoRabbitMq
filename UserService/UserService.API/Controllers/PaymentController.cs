using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs.Payment;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.UseCases;

namespace UserService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IEventPayment _eventPayment;

        public PaymentController(IEventPayment paymentRepository)
        {
            _eventPayment = paymentRepository;
        }

        [HttpPost]
        public async Task<ActionResult<CreatePaymentDto>> NewPaymentAsync(CreatePaymentDto request)
        {
            if(request == null)
            {
                return BadRequest();
            }

            var newPayment = await _eventPayment.ProcessPayment(request);
            if (!newPayment.Success)
            {
                return BadRequest(newPayment.ErrorMessage);
            }

            return Ok(newPayment);
        }
    }
}
