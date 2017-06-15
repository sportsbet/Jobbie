using Autofac;

namespace Jobbie.Sample.Client.WebApi.Host.Infrastructure.IoC
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