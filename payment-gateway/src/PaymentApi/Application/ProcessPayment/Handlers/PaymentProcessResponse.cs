namespace PaymentApi.Application.ProcessPayment.Handlers;

public class PaymentProcessResponse
{
    public string ResponseCode { get; set; }

    public string? InternalId { get; set; }
}