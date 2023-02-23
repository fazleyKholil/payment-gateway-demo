using Microsoft.Extensions.Logging;
using Payment.Common;
using Payment.Common.Dto;

namespace Infrastructure.BankConnection;

public class VisaBankConnectionSimulator : IBankConnection
{
    private readonly ILogger<VisaBankConnectionSimulator> _logger;
    private readonly IBankConnection _bankConnection;

    public VisaBankConnectionSimulator(ILogger<VisaBankConnectionSimulator> logger)
    {
        _logger = logger;
    }

    public async Task<BankResponseDto> ProcessPayment(BankRequestDto request)
    {
        return new BankResponseDto()
        {
            ResponseCode = ResponseCodes.BankProcessingApproved.ToString()
        };
    }
}