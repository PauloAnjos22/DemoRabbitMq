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
        private readonly IDepositFunds _depositFunds;

        public PaymentController(
            IEventPayment paymentRepository,
            IDepositFunds depositFunds)
        {
            _eventPayment = paymentRepository;
            _depositFunds = depositFunds;
        }

        [HttpPost("new-payment")]
        public async Task<ActionResult> NewPaymentAsync(CreatePaymentRequest request)
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

            return Ok();
        }
        [HttpPost("add-funds")]
        public async Task<ActionResult> DepositFundsAsync(CreateDepositRequest request)
        {
            var addDeposit = await _depositFunds.DepositFundsAsync(request);
            if (!addDeposit.Success) 
                return BadRequest(addDeposit.ErrorMessage);
            return Ok();
        }
    }
}
