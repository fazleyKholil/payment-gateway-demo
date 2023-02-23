using Azure.Security.KeyVault.Secrets;
using Infrastructure.Messaging.AzureStorageQueue;
using Infrastructure.Resiliency;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Vault.AzureVault;

public class AzureVaultService : IVaultService
{
    private readonly SecretClient _secretClient;
    private readonly ILogger<AzureStorageQueueService> _logger;

    public AzureVaultService(SecretClient secretClient,
        ILogger<AzureStorageQueueService> logger)
    {
        _secretClient = secretClient;
        _logger = logger;
    }

    public async Task<string?> AddAsync(string secretName, string value)
    {
        string? messageId = null;

        await PollyRetryRegistry.GetPolicyAsync(5, 2, "VaultAddAsync", _logger)
            .ExecuteAsync(async () =>
            {
                _logger.LogInformation($"Creating a secret {secretName}");

                var result = await _secretClient.SetSecretAsync(secretName, value);

                _logger.LogInformation($"Secret added with ID {result.Value.Value}.");

                messageId = result.Value.Value;
            });

        return messageId;
    }

    public async Task<string?> RetrieveAsync(string? secretName)
    {
        string? value = null;

        await PollyRetryRegistry.GetPolicyAsync(5, 2, "VaultRetrieveAsync", _logger)
            .ExecuteAsync(async () =>
            {
                Console.WriteLine($"Retrieving your secret {secretName}");
                var secret = await _secretClient.GetSecretAsync(secretName);
                Console.WriteLine($"Your secret is '{secret.Value.Value}'.");

                value = secret.Value.Value;
            });

        return value;
    }
}