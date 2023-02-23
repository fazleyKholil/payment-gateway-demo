using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Infrastructure.Vault;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using PaymentApi.Application.ProcessPayment.Commands;
using PaymentApi.Application.ProcessPayment.Handlers;
using PaymentApi.Domain.Models;

namespace PaymentApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IMediator _mediator;
    private readonly IVaultService _vaultService;

    public PaymentController(ILogger<PaymentController> logger,
        IMediator mediator,
        IVaultService vaultService)
    {
        _logger = logger;
        _mediator = mediator;
        _vaultService = vaultService;
    }

    [HttpPost(Name = "ProcessPayment")]
    public async Task<PaymentResponse> ProcessPayment([FromBody] PaymentRequest request)
    {
        _logger.LogInformation($"Receive Payment Request {request.TransactionId}");

        var response = await _mediator.Send(new PaymentProcessRequest
        {
            TransactionId = request.TransactionId,
            Amount = request.Amount,
            CardNumber = request.CardNumber,
            TransactionDate = request.TransactionDate
        });

        _logger.LogInformation($"Payment {request.TransactionId} processed with response {response.ResponseCode}");

        return new PaymentResponse
        {
            Response = response.ResponseCode
        };
    }
}