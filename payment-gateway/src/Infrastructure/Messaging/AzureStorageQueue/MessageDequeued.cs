using System.Text.Json;
using Azure.Storage.Queues.Models;
using MediatR;

namespace Infrastructure.Messaging.AzureStorageQueue;

public class QueueMessageEnvelope<T>
{
    public T Message { get; set; }

    public string MessageId { get; set; }

    public string PopReceipt { get; set; }
}

public class MessageDequeued<T> : IRequest<int?>, IRequest
{
    public List<QueueMessageEnvelope<T?>> Messages { get; set; }

    public MessageDequeued(List<QueueMessage> messages)
    {
        Messages = new List<QueueMessageEnvelope<T?>>();

        foreach (var message in messages)
        {
            var obj = JsonSerializer.Deserialize<T>(message.MessageText, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var queueMessage = new QueueMessageEnvelope<T?>
            {
                Message = obj,
                MessageId = message.MessageId,
                PopReceipt = message.PopReceipt
            };

            Messages.Add(queueMessage);
        }
    }
}