using System.Reflection;
using Autofac;
using Jobbie.Sample.Scheduler.Contracts.Api;
using Jobbie.Sample.Scheduler.Host.Infrastructure.Hypermedia;
using WebApi.Hal;
using Module = Autofac.Module;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.IoC
{
    internal sealed class HypermediaAutofacModule : Module
    {
        private static readonly Assembly _thisAssembly = Assembly.GetExecutingAssembly();

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(_thisAssembly)
                .AsClosedTypesOf(typeof(IHypermediaAppender<>))
                .InstancePerLifetimeScope()
                .AsImplementedInterfaces();

            builder
                .RegisterType<HypermediaConfiguration>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<PagedResourceAppender<ApiVersion>>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<PagedResourceAppender<Job>>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<PagedResourceAppender<Schedule>>()
                .AsImplementedInterfaces();
        }
    }
}