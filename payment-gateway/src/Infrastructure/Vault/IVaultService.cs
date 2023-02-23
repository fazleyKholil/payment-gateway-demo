namespace Infrastructure.Vault;

public interface IVaultService
{
    Task<string?> AddAsync(string secretName, string value);

    Task<string?> RetrieveAsync(string? secretName);
}