namespace Payment.Common.Dto;

public class BankRequestDto
{
    public string? CardNumber { get; set; }

    public decimal Amount { get; set; }
}