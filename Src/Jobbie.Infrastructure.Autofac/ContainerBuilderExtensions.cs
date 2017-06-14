using System.Reflection;
using Autofac;

namespace Jobbie.Infrastructure.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterBootstrappers(this ContainerBuilder builder, Assembly assembly) =>
            builder
                .RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IBootstrapper).IsAssignableFrom(t))
                .InstancePerLifetimeScope()
                .AsImplementedInterfaces();
    }
}