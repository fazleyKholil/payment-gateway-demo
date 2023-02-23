using MediatR;

namespace PaymentApi.Application.ProcessPayment.Handlers;

public class PaymentProcessRequest : IRequest<PaymentProcessResponse>
{
    public string TransactionId { get; set; }

    public decimal Amount { get; set; }

    public string CardNumber { get; set; }

    public DateTime TransactionDate { get; set; }
}