using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.Repositories.Interfaces;

public abstract class BaseRepository<T> : IBaseRepository<T>
{
    protected readonly DbConnectionOptions _connectionOptions;
    protected readonly ILogger<BaseRepository<T>> _logger;

    protected BaseRepository(ILogger<BaseRepository<T>> logger,
        IOptions<DbConnectionOptions> connectionOptions)
    {
        _logger = logger;
        _connectionOptions = connectionOptions.Value;
    }


    public async Task<int> AddAsync(T en)
    {
        try
        {
            _logger.LogInformation($"Persisting {typeof(T)} To DB");

            await using var conn = new SqlConnection(_connectionOptions.ConnectionString);
            return (await conn.InsertAsync(en)).Value;
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occured while persisting {typeof(T)} to DB", e);
            throw;
        }
    }

    public async Task<int> UpdateAsync(T en)
    {
        try
        {
            _logger.LogInformation($"Updating {typeof(T)} To DB");

            await using var conn = new SqlConnection(_connectionOptions.ConnectionString);
            return (await conn.UpdateAsync(en));
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occured while updating {typeof(T)} from DB", e);
            throw;
        }
    }

    public async Task<int> DeleteAsync(T en)
    {
        try
        {
            _logger.LogInformation($"Deleting {typeof(T)}");

            await using var conn = new SqlConnection(_connectionOptions.ConnectionString);
            return await conn.DeleteAsync(en);
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occured while deleting {typeof(T)} from DB", e);
            throw;
        }
    }

    public async Task<IEnumerable<T>> GetListAsync(string condition, object param)
    {
        try
        {
            _logger.LogInformation($"Fetching records {typeof(T)} from DB");

            await using var conn = new SqlConnection(_connectionOptions.ConnectionString);
            return await conn.GetListAsync<T>(condition, param);
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occured while fetching records of {typeof(T)} from DB", e);
            throw;
        }
    }
}