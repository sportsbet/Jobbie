using Common.Logging;
using Common.Logging.Simple;
using Jobbie.Sample.Scheduler.Host.Infrastructure.IoC;
using Topshelf;
using Topshelf.Autofac;

namespace Jobbie.Sample.Scheduler.Host
{
    internal sealed class Program
    {
        private static void Main()
        {
            ConfigureLogging();

            HostFactory.Run(
                h =>
                {
                    h.SetServiceName("Jobbie.Sample.Scheduler.Host");
                    h.SetDisplayName("Jobbie.Sample.Scheduler.Host");

                    h.UseAutofacContainer(IoCBootstrapper.Init());

                    h.Service<HostService>(
                        s =>
                        {
                            s.ConstructUsingAutofacContainer();
                            s.WhenStarted(x => x.Start());
                            s.WhenStopped(x => x.Stop());
                        });
                });
        }

        private static void ConfigureLogging() =>
            LogManager.Adapter =
                new ConsoleOutLoggerFactoryAdapter(LogLevel.Info, true, true, true, "yyyy-MM--dd HH:mm:ss:fff", true);
    }
}
