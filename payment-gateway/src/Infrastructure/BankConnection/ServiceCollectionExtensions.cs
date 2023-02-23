using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.BankConnection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBankSimulator(this IServiceCollection services)
    {
        services.AddSingleton<IBankConnection, VisaBankConnectionSimulator>();

        return services;
    }
}