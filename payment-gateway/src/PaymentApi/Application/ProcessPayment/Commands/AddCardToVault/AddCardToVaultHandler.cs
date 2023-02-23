using Infrastructure.Vault;
using MediatR;

namespace PaymentApi.Application.ProcessPayment.Commands.AddCardToVault;

public class AddCardToVaultHandler : IRequestHandler<AddCardToVaultCommand>
{
    private readonly ILogger<AddCardToVaultHandler> _logger;
    private readonly IVaultService _vaultService;

    public AddCardToVaultHandler(ILogger<AddCardToVaultHandler> logger,
        IVaultService vaultService)
    {
        _logger = logger;
        _vaultService = vaultService;
    }

    public async Task Handle(AddCardToVaultCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            $"Adding Card start with {request.CardNumber[..6]} with VaultID {request.VaultId} to Vault");

        var res = await _vaultService.AddAsync(request.VaultId, request.CardNumber);

        _logger.LogInformation($"Card added successfully with ID {res}");
    }
}