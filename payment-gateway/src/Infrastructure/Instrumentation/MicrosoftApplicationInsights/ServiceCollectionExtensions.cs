using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Instrumentation.MicrosoftApplicationInsights;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMicrosoftApplicationInsights(
        this IServiceCollection services,
        string roleName,
        string roleInstance)
    {
        services.AddApplicationInsightsTelemetry();
        services.AddSingleton<ITelemetryInitializer>(new CustomTelemetryInitializer(roleName, roleInstance));

        return services;
    }
}