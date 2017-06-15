using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Autofac;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.IoC
{
    internal sealed class HttpConfigurationAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterInstance(new HttpConfiguration())
                .AsSelf()
                .SingleInstance();

            builder
                .Register(c => c.Resolve<HttpConfiguration>().Routes)
                .AsSelf();

            builder
                .Register(c => c.Resolve<HttpConfiguration>().Filters)
                .AsSelf();

            builder
                .Register(c => c.Resolve<HttpConfiguration>().Formatters)
                .AsSelf();

            builder
                .Register(
                    c =>
                    {
                        var httpContext = HttpContext.Current;
                        if (HttpContext.Current == null)
                            httpContext = new HttpContext(c.Resolve<SimpleWorkerRequest>());
                        return new HttpContextWrapper(httpContext);
                    })
                .As<HttpContextBase>();

            builder
                .Register(c => c.Resolve<HttpContextBase>().Request)
                .AsSelf();

            builder
                .Register(c => c.Resolve<HttpRequestBase>().RequestContext)
                .AsSelf();

            builder
                .Register(c => new SimpleWorkerRequest(string.Empty, string.Empty, new StringWriter()))
                .AsSelf();
        }
    }
}