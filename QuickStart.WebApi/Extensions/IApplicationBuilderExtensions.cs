namespace Microsoft.AspNetCore.Builder;

public static class IApplicationBuilderExtensions
{
    public static void RegisterJobs(this IApplicationBuilder app)
    {
        var backgroundService = app.ApplicationServices.GetService<BackgroundJobService>();

        backgroundService?.EnqueueHelloWorld();
    }
}
