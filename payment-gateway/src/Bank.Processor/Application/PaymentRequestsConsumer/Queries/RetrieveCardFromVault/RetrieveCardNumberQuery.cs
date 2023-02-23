using MediatR;

namespace Bank.Processor.Application.PaymentRequestsConsumer.Queries.RetrieveCardFromVault;

public class RetrieveCardNumberQuery : IRequest<CardNumberDto>
{
    public string? VaultId { get; set; }
}