namespace QuickStart.WebApi.Services;

public class BackgroundJobService
{
    private readonly ILogger<BackgroundJobService> _logger;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public BackgroundJobService(ILogger<BackgroundJobService> logger, IBackgroundJobClient backgroundJobClient)
    {
        _logger = logger;
        _backgroundJobClient = backgroundJobClient;
    }

    public void EnqueueHelloWorld()
    {
        _logger.LogInformation("The hello world job enqueued!");
        //_backgroundJobClient.Enqueue(() => System.Diagnostics.Trace.WriteLine("Hello world!"));
        _backgroundJobClient.Enqueue(() => Console.WriteLine("Hello world!"));
    }
}