namespace QuickStart.ConsoleApp;

internal class ConsoleJobs
{
    public void HelloWorld()
    {
        Thread.Sleep(5000);
        Console.WriteLine("Hello World From Hangfire Console!");
    }
}