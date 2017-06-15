using Jobbie.Sample.Client.WebApi.Host.Infrastructure.IoC;
using Topshelf;
using Topshelf.Autofac;

namespace Jobbie.Sample.Client.WebApi.Host
{
    internal sealed class Program
    {
        private static void Main()
        {
            var container = IoCBootstrapper.Init();

            HostFactory.Run(
                h =>
                {
                    h.SetServiceName("Jobbie.Sample.Client.WebApi.Host");
                    h.SetDisplayName("Jobbie.Sample.Client.WebApi.Host");

                    h.UseAutofacContainer(container);

                    h.Service<HostService>(
                        s =>
                        {
                            s.ConstructUsingAutofacContainer();
                            s.WhenStarted(x => x.Start());
                            s.WhenStopped(x => x.Stop());
                        });
                });
        }
    }
}
