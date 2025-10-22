using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Events
{
    public class TransactionCompletedEvent
    {
        public Guid TransactionId { get; init; }
        public Guid AccountFrom { get; init; }
        public Guid AccountTo { get; init; }
        public long PaymentAmount { get; init; }
        public string Currency { get; init; }
        public string? PaymentMethod { get; init; }
        public string? Status { get; init; }
        public string? FailureReason { get; set; }

        public DateTime CreatedAt { get; init; }
        public DateTime CompletedAt { get; init; }


        public TransactionCompletedEvent(Guid transactionId, Guid accountFrom, Guid accountTo, long paymentAmount, string currency, string? paymentMethod, string? status, string? failureReason = null)
        {
            TransactionId = transactionId;
            AccountFrom = accountFrom;
            AccountTo = accountTo;
            PaymentMethod = paymentMethod;
            PaymentAmount = paymentAmount;
            Currency = string.IsNullOrWhiteSpace(currency) ? "BRL" : currency;
            Status = status;
            FailureReason = failureReason;
            CreatedAt = DateTime.Now;
            CompletedAt = DateTime.Now;
        }
    }
}
