using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs.Common;
using UserService.Application.DTOs.Payment;

namespace UserService.Application.Interfaces.Services
{
    public interface IPaymentValidator
    {
        Task<ResultResponse> ValidateAsync(CreatePaymentRequest request);
    }
}
