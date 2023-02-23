using System.Security.AccessControl;

namespace Infrastructure.Messaging.AzureStorageQueue;

public class BaseMessageDequeued
{
    public string messageId { get; set; }

    public string popReceipt { get; set; }
}