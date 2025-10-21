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
        public int PaymentAmount { get; init; }
        public string Currency { get; init; }
        public string? PaymentMethod { get; init; }
        public string? Status { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime CompletedAt { get; init; }


        public TransactionCompletedEvent(Guid transactionId, Guid accountFrom, Guid accountTo, int paymentAmount, string currency, string? paymentMethod, string? status)
        {
            TransactionId = transactionId;
            AccountFrom = accountFrom;
            AccountTo = accountTo;
            PaymentMethod = paymentMethod;
            PaymentAmount = paymentAmount;
            Currency = string.IsNullOrWhiteSpace(currency) ? "BRL" : currency;
            CreatedAt = DateTime.Now;
            CompletedAt = DateTime.Now;
        }
    }
}
