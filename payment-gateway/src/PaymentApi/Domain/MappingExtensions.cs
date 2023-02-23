using Payment.Common.Dto;
using Payment.Common.Entities;
using PaymentApi.Application.ProcessPayment.Commands;
using PaymentApi.Application.ProcessPayment.Handlers;
using PaymentApi.Domain.Models;

namespace PaymentApi.Domain;

public static class MappingExtensions
{
    public static PaymentRequestDto ToDto(this PaymentProcessRequest request, string vaultId)
    {
        var dto = new PaymentRequestDto()
        {
            TransactionDate = request.TransactionDate,
            Amount = request.Amount,
            TransactionId = request.TransactionId,
            VaultId = vaultId
        };

        return dto;
    }

    public static PaymentRequestEntity ToEntity(this PaymentProcessRequest request, string vaultId)
    {
        var en = new PaymentRequestEntity
        {
            Amount = request.Amount,
            TransactionId = request.TransactionId,
            VaultId = vaultId,
            TransactionDate = request.TransactionDate,
            CreatedDate = DateTime.Now
        };

        return en;
    }

    public static PaymentResponseEntity ToEntity(this PaymentProcessResponse response, PaymentRequestEntity request)
    {
        var en = new PaymentResponseEntity
        {
            CreatedDate = DateTime.Now,
            Response = response.ResponseCode,
            PaymentId = request.PaymentId
        };

        return en;
    }
}