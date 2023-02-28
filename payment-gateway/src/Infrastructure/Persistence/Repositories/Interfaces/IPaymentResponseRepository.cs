using Payment.Common.Entities;

namespace Infrastructure.Persistence.Repositories.Interfaces;

public interface IPaymentResponseRepository: IBaseRepository<PaymentResponseEntity>
{
    Task UpdatePaymentResponse(string transactionId, string response);
}