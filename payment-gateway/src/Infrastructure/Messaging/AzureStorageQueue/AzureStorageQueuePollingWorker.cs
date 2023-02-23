using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Messaging.AzureStorageQueue;

public class AzureStorageQueuePollingWorker<T> : IHostedService, IDisposable
{
    private readonly IMediator _mediator;
    private readonly IQeueingService _qeueingService;
    private readonly QueueingOptions _queueingOptions;
    private readonly ILogger<AzureStorageQueueService> _logger;
    private Timer? _timer = null;

    public AzureStorageQueuePollingWorker(
        IMediator mediator,
        IOptions<QueueingOptions> queueOptions,
        IQeueingService qeueingService,
        ILogger<AzureStorageQueueService> logger)
    {
        _mediator = mediator;
        _queueingOptions = queueOptions.Value;
        _qeueingService = qeueingService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Azure Storage Queue Poll Worker Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(10));

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        _logger.LogInformation($"Start Polling from Queue");

        var messages = await _qeueingService.ReceiveMessageAsync();

        if (messages.Count > 0)
            await _mediator.Send(new MessageDequeued<T>(messages));

        _logger.LogInformation($"{messages.Count} messages was Dequeued");
    }


    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}