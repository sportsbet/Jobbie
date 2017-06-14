using Autofac;

namespace Jobbie.Sample.Executor.Host.Infrastructure.IoC
{
    internal sealed class HostAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<HostService>()
                .AsSelf();
        }
    }
}