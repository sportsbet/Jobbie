using System.Collections.Generic;
using Autofac;

namespace Jobbie.Infrastructure.Autofac
{
    public static class ComponentContextExtensions
    {
        public static void InitBootstrappers(this IComponentContext container)
        {
            var bootstrappers = container.Resolve<IEnumerable<IBootstrapper>>();
            foreach (var bootstrapper in bootstrappers)
                bootstrapper.Init();
        }
    }
}