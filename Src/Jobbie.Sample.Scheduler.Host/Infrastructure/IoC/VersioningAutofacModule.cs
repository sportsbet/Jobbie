using Autofac;
using Jobbie.Sample.Scheduler.Contracts.Api;
using Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.IoC
{
    internal sealed class VersioningAutofacModule : Module
    {
        private static readonly IApiVersion[] _versions =
        {
            new ApiVersion("1.0")
        };

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(c => c.Resolve<ISupportedApiVersions>().Latest)
                .As<ILatestApiVersion>()
                .SingleInstance();

            builder
                .RegisterType<ApiVersionRouteConstraint>()
                .AsSelf();

            builder
                .RegisterType<ApiVersionFromRequestRouteReader>()
                .AsImplementedInterfaces();

            builder
                .Register(c => c.Resolve<IApiVersionReader>().Read())
                .As<ICurrentApiVersion>();

            builder
                .Register(c => new SupportedApiVersions(_versions))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}