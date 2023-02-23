using Payment.Common.Dto;

namespace Infrastructure.BankConnection;

public interface IBankConnection
{
    Task<BankResponseDto> ProcessPayment(BankRequestDto request);
}