using System.Net.Http;
using Autofac;

namespace Jobbie.Sample.Executor.Host.Infrastructure.IoC
{
    internal sealed class HttpClientAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<HttpClient>()
                .AsSelf();
        }
    }
}