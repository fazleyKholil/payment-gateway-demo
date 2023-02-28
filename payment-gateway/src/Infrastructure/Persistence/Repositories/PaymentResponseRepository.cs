using System.Data.SqlClient;
using Dapper;
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

    public async Task UpdatePaymentResponse(string transactionId, string response)
    {
        try
        {
            _logger.LogInformation($"Updating TransactionId {transactionId} Status");

            await using var conn = new SqlConnection(_connectionOptions.ConnectionString);
            await conn.ExecuteAsync(@"
                   UPDATE R 
                   SET R.Response = @Response 
                   FROM dbo.PaymentResponse AS R
                   INNER JOIN dbo.PaymentRequest AS P 
                   ON R.PaymentId = P.PaymentId 
                   WHERE P.TransactionId = @TransactionId
                ", new { Response = response, TransactionId = transactionId });
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occured while updating  TransactionId {transactionId} from DB", e);
            throw;
        }
    }
}