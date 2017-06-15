using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using Jobbie.Infrastructure.Autofac;

namespace Jobbie.Sample.Client.WebApi.Host.Infrastructure.IoC
{
    internal static class IoCBootstrapper
    {
        private static readonly Assembly _thisAssembly = Assembly.GetExecutingAssembly();
        public static IContainer Container;

        public static IContainer Init()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(_thisAssembly);

            builder.RegisterBootstrappers(_thisAssembly);

            builder.RegisterAssemblyModules(_thisAssembly);

            Container = builder.Build();

            Container.InitBootstrappers();

            return Container;
        }
    }
}