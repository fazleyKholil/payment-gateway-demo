using Azure.Storage.Queues;
using Infrastructure.Messaging;
using Infrastructure.Messaging.AzureStorageQueue;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentRequestRepository, PaymentRequestRepository>();
        services.AddSingleton<IPaymentResponseRepository, PaymentResponseRepository>();

        return services;
    }
}