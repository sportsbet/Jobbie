namespace Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning
{
    internal interface IApiVersionReader
    {
        ICurrentApiVersion Read();
    }
}