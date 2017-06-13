using System.Diagnostics;
using System.Net.Http;
using Autofac;
using Jobbie.Infrastructure.Http;
using Jobbie.Infrastructure.Models;
using Jobbie.Infrastructure.Queries;

namespace Jobbie.Infrastructure.Autofac
{
    public sealed class JobbieInfrastructureAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterModels(builder);
            RegisterQueries(builder);
            RegisterHttp(builder);
        }

        private static void RegisterModels(ContainerBuilder builder)
        {
            builder
                .RegisterType<Now>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<Stopwatch>()
                .AsSelf();
        }

        private static void RegisterQueries(ContainerBuilder builder)
        {
            builder
                .RegisterType<JobConverter>()
                .AsImplementedInterfaces();
        }

        private static void RegisterHttp(ContainerBuilder builder)
        {
            builder
                .RegisterType<HttpClient>()
                .AsSelf();

            builder
                .RegisterType<HttpClientWrapper>()
                .AsImplementedInterfaces();
        }
    }
}