using Infrastructure.Messaging;
using Infrastructure.Persistence.Repositories.Interfaces;
using MediatR;
using Payment.Common;
using PaymentApi.Application.ProcessPayment.Commands.AddCardToVault;
using PaymentApi.Domain;

namespace PaymentApi.Application.ProcessPayment.Handlers;

public class PaymentHandler : IRequestHandler<PaymentProcessRequest, PaymentProcessResponse?>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentHandler> _logger;
    private IQeueingService _qeueingService;
    private IPaymentRequestRepository _paymentRequestRepository;
    private IPaymentResponseRepository _paymentResponseRepository;

    public PaymentHandler(IMediator mediator,
        ILogger<PaymentHandler> logger,
        IQeueingService qeueingService,
        IPaymentRequestRepository paymentRequestRepository,
        IPaymentResponseRepository paymentResponseRepository)
    {
        _mediator = mediator;
        _logger = logger;
        _qeueingService = qeueingService;
        _paymentRequestRepository = paymentRequestRepository;
        _paymentResponseRepository = paymentResponseRepository;
    }

    public async Task<PaymentProcessResponse?> Handle(PaymentProcessRequest processRequest,
        CancellationToken cancellationToken)
    {
        var paymentResponse = new PaymentProcessResponse();

        try
        {
            _logger.LogInformation($"Processing transaction with Id {processRequest.TransactionId}");

            var vaultId = Guid.NewGuid().ToString();

            //persists payment request
            var paymentRequestEntity = processRequest.ToEntity(vaultId);
            var paymentId = await _paymentRequestRepository.AddAsync(paymentRequestEntity);
            paymentRequestEntity.PaymentId = paymentId;

            //save card safely to vault
            await _mediator.Send(new AddCardToVaultCommand()
            {
                CardNumber = processRequest.CardNumber,
                VaultId = vaultId
            }, cancellationToken);

            // Add payment to queue for offline bank confirmation
            var messageId = await _qeueingService.InsertAsync(processRequest.ToDto(vaultId));

            paymentResponse.InternalId = messageId;
            paymentResponse.ResponseCode = ResponseCodes.BankProcessingPending.ToString();

            //persists payment response
            await _paymentResponseRepository.AddAsync(paymentResponse.ToEntity(paymentRequestEntity));

            return paymentResponse;
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occured while processing transaction with ID : {processRequest.TransactionId}",
                e);
            throw;
        }
    }
}