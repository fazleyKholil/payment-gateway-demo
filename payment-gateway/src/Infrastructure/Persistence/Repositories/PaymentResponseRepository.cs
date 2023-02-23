using Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment.Common.Entities;

namespace Infrastructure.Persistence.Repositories;

public class PaymentResponseRepository : BaseRepository<PaymentResponseEntity>, IPaymentResponseRepository
{
    public PaymentResponseRepository(ILogger<BaseRepository<PaymentResponseEntity>> logger,
        IOptions<DbConnectionOptions> connectionOptions)
        : base(logger, connectionOptions)
    {
    }
}