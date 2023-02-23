using Microsoft.Extensions.Logging;
using Polly;


namespace Infrastructure.Resiliency;

public class PollyRetryRegistry
{
    public static AsyncPolicy GetPolicyAsync(int retryCount, int incrementalCount, string policyKey, ILogger logger)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(retryCount,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt * incrementalCount),
                onRetry: (exception, pollyRetryCount, context) =>
                {
                    logger.LogError(exception,
                        $@"An error has occured at {exception.Source} with message {exception.Message}
                                for context {context.PolicyKey}. Waiting {pollyRetryCount.TotalSeconds.ToString()} seconds for next retry.");
                }
            )
            .WithPolicyKey(policyKey);
    }

    public static Policy GetPolicy(int retryCount, int incrementalCount, string policyKey, ILogger logger)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetry(retryCount,
                retryAttempt => TimeSpan.FromSeconds(retryAttempt * incrementalCount),
                onRetry: (exception, pollyRetryCount, context) =>
                {
                    logger.LogError(exception,
                        $@"An error has occured at {exception.Source} with message {exception.Message}
                                for context {context.PolicyKey}. Waiting {pollyRetryCount.TotalSeconds.ToString()} seconds for next retry.");
                }
            )
            .WithPolicyKey(policyKey);
    }
}