using MediatR;

namespace PaymentApi.Application.ProcessPayment.Commands.AddCardToVault;

public class AddCardToVaultCommand : IRequest
{
    public string VaultId { get; set; }

    public string CardNumber { get; init; }
}