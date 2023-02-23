using Azure.Identity;
using Infrastructure.Vault.AzureVault;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Vault;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureSecrets(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddSecretClient(configuration.GetSection("KeyVault"));
            clientBuilder.UseCredential(new DefaultAzureCredential());
        });

        services.AddSingleton<IVaultService, AzureVaultService>();

        return services;
    }
}