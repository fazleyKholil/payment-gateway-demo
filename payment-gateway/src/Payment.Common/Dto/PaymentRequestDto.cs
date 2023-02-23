namespace Payment.Common.Dto;

public class PaymentRequestDto
{
    public string TransactionId { get; set; }

    public decimal Amount { get; set; }

    public string? VaultId { get; set; }

    public DateTime TransactionDate { get; set; }
}