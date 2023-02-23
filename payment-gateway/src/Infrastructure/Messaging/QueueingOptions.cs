namespace Infrastructure.Messaging;

public class QueueingOptions
{
    public string QueueConnection { get; set; }

    public string QueueName { get; set; }
    
    public int BatchCount { get; set; }
}