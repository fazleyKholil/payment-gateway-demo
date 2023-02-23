using Azure.Storage.Queues;
using Infrastructure.Instrumentation.MicrosoftApplicationInsights;
using Infrastructure.Messaging.AzureStorageQueue;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Common.Dto;

namespace Infrastructure.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorageQueue(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAzureClients(builder =>
            {
                builder.AddClient<QueueClient, QueueClientOptions>((_, _, _) =>
                {
                    var connectionString = configuration.GetSection("Queueing:QueueConnection").Value;
                    var queueName = configuration.GetSection("Queueing:QueueName").Value;
                    return new QueueClient(connectionString, queueName);
                });
            });

        services.AddSingleton<IQeueingService, AzureStorageQueueService>();

        return services;
    }

    public static IServiceCollection AddAzureStorageQueueConsumer<T>(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<AzureStorageQueuePollingWorker<T>>();

        return services;
    }
}