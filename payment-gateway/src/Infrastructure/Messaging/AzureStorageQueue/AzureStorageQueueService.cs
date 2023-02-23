using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Infrastructure.Resiliency;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Messaging.AzureStorageQueue;

public class AzureStorageQueueService : IQeueingService
{
    private readonly QueueClient _queueClient;
    private readonly QueueingOptions _queueingOptions;
    private readonly ILogger<AzureStorageQueueService> _logger;

    public AzureStorageQueueService(IOptions<QueueingOptions> queueOptions,
        QueueClient queueClient,
        ILogger<AzureStorageQueueService> logger)
    {
        _queueingOptions = queueOptions.Value;
        _queueClient = queueClient;
        _logger = logger;
    }

    public async Task<string?> InsertAsync<T>(T message)
    {
        string? messageId = null;

        await PollyRetryRegistry.GetPolicyAsync(5, 2, "InsertAsync", _logger)
            .ExecuteAsync(async () =>
            {
                _logger.LogInformation($"Inserting message {message} to queue");
                var response = await _queueClient.SendMessageAsync(JsonSerializer.Serialize(message,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }));
                _logger.LogInformation($"Message Inserted with messageId {response.Value.MessageId}");

                messageId = response.Value.MessageId;
            });

        return messageId;
    }

    public async Task<List<QueueMessage>> ReceiveMessageAsync()
    {
        var maxCount = _queueingOptions.BatchCount;
        var maxBatchCount = 32;
        var receivedMessages = new List<QueueMessage>();

        do
        {
            if (maxCount < 32)
                maxBatchCount = maxCount;
            QueueMessage[] messages = await _queueClient.ReceiveMessagesAsync(maxBatchCount, TimeSpan.FromSeconds(30));
            receivedMessages.AddRange(messages);

            if (messages.Count() < maxBatchCount)
                return receivedMessages;

            maxCount -= messages.Count();
        } while (maxCount > 0);

        return receivedMessages;
    }

    public async Task HandleMessageAsync(string messageId, string popReceipt)
    {
        await _queueClient.DeleteMessageAsync(messageId, popReceipt);
    }
}