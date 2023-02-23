using Infrastructure.Vault;
using MediatR;

namespace Bank.Processor.Application.PaymentRequestsConsumer.Queries.RetrieveCardFromVault;

public class RetrieveCardNumberHandler : IRequestHandler<RetrieveCardNumberQuery, CardNumberDto>
{
    private readonly ILogger<RetrieveCardNumberHandler> _logger;
    private readonly IVaultService _vaultService;

    public RetrieveCardNumberHandler(ILogger<RetrieveCardNumberHandler> logger,
        IVaultService vaultService)
    {
        _logger = logger;
        _vaultService = vaultService;
    }


    public async Task<CardNumberDto> Handle(RetrieveCardNumberQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Retrieving Card Number with Vault Id {request.VaultId} from vault");

        var cardNumber = await _vaultService.RetrieveAsync(request.VaultId);

        _logger.LogInformation($"Card Number with Vault Id {request.VaultId} retrieved successfully");

        return new CardNumberDto
        {
            DecryptedCardNumber = cardNumber
        };
    }
}