string? env = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{env}.json", true, true)
    .Build();

string connectionString = configuration.GetConnectionString("HangfireConnection");

//使用 SQL Server, 可以指定配置文件中的连接字符串名称或者是具体的连接字符串内容
GlobalConfiguration.Configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSerilogLogProvider()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        //SqlClientFactory = System.Data.SqlClient.SqlClientFactory或SqlClientFactory = Microsoft.Data.SqlClient.SqlClientFactory Hangfire会自动根据引用的包进行确认，如果两个都有引用，则可以显式设置
        PrepareSchemaIfNecessary = false,//手动处理Hangfire数据表
        TryAutoDetectSchemaDependentOptions = false, //默认是true 如果想避免在启动期间进行网络调用，可以通过此配置设置
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,//任务轮询时间,当设置 SlidingInvisibilityTimeout 选项时，可以使用 TimeSpan.Zero 作为轮询间隔
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    });

BackgroundJob.Enqueue(() => new ConsoleJobs().HelloWorld());

using (var server = new BackgroundJobServer())
{
    Console.ReadLine();
}
/*
 * https://docs.hangfire.io/en/latest/configuration/using-sql-server.html

 * https://discuss.hangfire.io/t/how-does-the-following-sqlserverstorageoptions-work/6127

 * SlidingInvisibilityTimeout is used to indicate how long a BackgroundJob execution is allowed to run for without status change (success/failure) before Hangfire decides the BackgroundJob execution was not successful and needs to be made visible to the HangfireServer for processing again. The idea being that if a HangfireServer starts processing a BackgroundJob and then gets killed without being able to report back that the BackgroundJob failed then there would need to be a retry of that BackgroundJob.
 * You probably shouldn’t change SlidingInvisibilityTimeout unless you have a really compelling reason. For example, if you set it to 5 minutes and then have a BackgroundJob that runs for 6 minutes, Hangfire is going to end up queuing up multiple instances of that BackgroundJob because it hasn’t completed fast enough.
 * QueuePollInterval is how long the server is going to wait in between checking the database for new BackgroundJobs to process. Setting this value will result in Jobs being picked up off the queue faster, but also more load on your SQL Server.
 * If you need faster dequeue time, utilizing MSMQ alongside SQL Server Storage can get near to real-time without the overhead of more polling: https://docs.hangfire.io/en/latest/configuration/using-sql-server-with-msmq.html 

 * SlidingInvisibilityTimeout changes Hangfire’s strategy of how to determine whether a background job that a worker dequeued is still alive and being processed. Rather than keeping a DB connection and transaction active while the job is running (thus locking the job for other workers), the worker that processed the job will periodically update a timestamp in the DB. Only when the timestamp hasn’t been updated for some time (the value set as the “sliding invisibility timeout”), the job becomes abandoned. (This is a bit different than what Jonah writes above - even when the job runs for longer than the timeout, it will not be abandoned as long as the the worker can still update the timestamp, signaling that it’s still working on the job.)
 * When the QueuePollInterval is set to zero, Hangfire will switch to a sort of “long polling”, where a single long-running query is executed that will periodically check and wait on the server side (via WAITFOR DELAY) until a new job arrives. This reduces the DB load caused by client side polling.
 * Setting a CommandBatchMaxTimeout will enable a strategy where Hangfire sends multiple queries in a single batch, reducing DB roundtrips (and increasing concurrency).
 * DisableGlobalLocks will turn off some global SQL Server application locks, which are no longer deemed necessary with schema version 7. (With schema version 6, deadlocks can arise when turning off the global locks.)
 */