namespace Jobbie.Sample.Client.WebApi.Host.Contracts
{
    public sealed class JobExecution
    {
        public string Description { get; set; }
        public string Cron { get; set; }

        public override string ToString() => $"Description={Description}|Cron={Cron}";
    }
}