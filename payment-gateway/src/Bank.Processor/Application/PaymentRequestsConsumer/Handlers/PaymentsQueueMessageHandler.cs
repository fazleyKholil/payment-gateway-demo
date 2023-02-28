using Bank.Processor.Application.PaymentRequestsConsumer.Queries.RetrieveCardFromVault;
using Infrastructure.BankConnection;
using Infrastructure.Messaging;
using Infrastructure.Messaging.AzureStorageQueue;
using Infrastructure.Persistence.Repositories.Interfaces;
using MediatR;
using Payment.Common;
using Payment.Common.Dto;

namespace Bank.Processor.Application.PaymentRequestsConsumer.Handlers;

public class PaymentsQueueMessageHandler : IRequestHandler<MessageDequeued<PaymentRequestDto>>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsQueueMessageHandler> _logger;
    private readonly IPaymentResponseRepository _paymentResponseRepository;
    private readonly IBankConnection _bankConnection;
    private IQeueingService _qeueingService;

    public PaymentsQueueMessageHandler(IMediator mediator,
        IBankConnection bankConnection,
        IQeueingService qeueingService,
        IPaymentResponseRepository paymentResponseRepository,
        ILogger<PaymentsQueueMessageHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _qeueingService = qeueingService;
        _paymentResponseRepository = paymentResponseRepository;
        _bankConnection = bankConnection;
    }

    public async Task Handle(MessageDequeued<PaymentRequestDto> processRequest,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processing batch of payments");

        var paymentRequests = processRequest;

        foreach (var message in paymentRequests.Messages)
        {
            var paymentReq = message.Message;

            try
            {
                _logger.LogInformation($"Decrypting cardnumber with VaultId {paymentReq?.VaultId}");

                //decrypt the card number
                var cardResult = await _mediator.Send(new RetrieveCardNumberQuery()
                {
                    VaultId = paymentReq?.VaultId
                }, cancellationToken);

                _logger.LogInformation(
                    $"Calling bank service for approval for TransactionId {paymentReq?.TransactionId}");

                //get approval from external bank service
                var bankResponse = await _bankConnection.ProcessPayment(new BankRequestDto()
                {
                    CardNumber = cardResult.DecryptedCardNumber,
                    Amount = paymentReq.Amount
                });

                _logger.LogInformation(
                    $"Bank returned {bankResponse.ResponseCode} for TransactionId {paymentReq.TransactionId} ");

                //update Payment status
                await _paymentResponseRepository.UpdatePaymentResponse(paymentReq.TransactionId,
                    ResponseCodes.BankProcessingApproved.ToString());

                //emit events to notify merchant back event grid / sns --> webhooks

                //delete message from queue
                await _qeueingService.HandleMessageAsync(message.MessageId, message.PopReceipt);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occured while processing transaction with ID : {paymentReq?.TransactionId} {e.Message}",
                    e);
            }
        }
    }
}