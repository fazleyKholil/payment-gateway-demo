namespace PaymentApi.Domain.Models;

public class PaymentRequest
{
    public string TransactionId { get; set; }

    public decimal Amount { get; set; }

    public string CardNumber { get; set; }

    public DateTime TransactionDate { get; set; }
}