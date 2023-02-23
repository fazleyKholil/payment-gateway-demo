using Azure.Storage.Queues.Models;

namespace Infrastructure.Messaging;

public interface IQeueingService
{
    Task<string?> InsertAsync<T>(T message);

    Task<List<QueueMessage>> ReceiveMessageAsync();

    Task HandleMessageAsync(string messageId, string popReceipt);
}