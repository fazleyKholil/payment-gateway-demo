using Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment.Common.Entities;

namespace Infrastructure.Persistence.Repositories;

public class PaymentRequestRepository : BaseRepository<PaymentRequestEntity>, IPaymentRequestRepository
{
    public PaymentRequestRepository(ILogger<BaseRepository<PaymentRequestEntity>> logger,
        IOptions<DbConnectionOptions> connectionOptions)
        : base(logger, connectionOptions)
    {
    }
}