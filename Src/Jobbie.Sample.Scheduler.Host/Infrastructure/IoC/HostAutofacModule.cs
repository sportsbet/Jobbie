using Autofac;
using Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Hosting;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.IoC
{
    internal sealed class HostAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<HostService>()
                .AsSelf();

            builder
                .RegisterType<PublicHostConfiguration>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<InternalHostConfiguration>()
                .AsImplementedInterfaces();
        }
    }
}