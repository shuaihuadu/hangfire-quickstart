namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, string connectionString)
    {
        //Add Hangfire services.
        services.AddHangfire(configuration =>
        {
            configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString);
        });

        //Add the processing server as IHostedService
        services.AddHangfireServer();

        return services;
    }

    public static IServiceCollection AddBackgroundJobService(this IServiceCollection services)
    {
        services.AddSingleton<BackgroundJobService>();

        return services;
    }
}
