using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Jobbie.Sample.Client.WebApi.Host.Infrastructure.IoC;
using Owin;

namespace Jobbie.Sample.Client.WebApi.Host.Infrastructure.WebApi.Hosting
{
    internal sealed class OwinConfiguration
    {
        public void Configuration(IAppBuilder builder)
        {
            var container = IoCBootstrapper.Container;
            var http = container.Resolve<HttpConfiguration>();

            http.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            builder.UseAutofacMiddleware(container);
            builder.UseAutofacWebApi(http);
            builder.UseWebApi(http);
        }
    }
}