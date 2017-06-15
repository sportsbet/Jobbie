using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using FluentValidation.WebApi;
using Jobbie.Sample.Scheduler.Host.Infrastructure.IoC;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Hosting
{
    internal sealed class OwinConfiguration
    {
        public void Configuration(IAppBuilder builder)
        {
            var container = IoCBootstrapper.Container;
            var http = container.Resolve<HttpConfiguration>();

            http.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            builder.UseCors(CorsOptions.AllowAll);
            builder.UseAutofacMiddleware(container);
            builder.UseAutofacWebApi(http);
            builder.UseWebApi(http);

            var options =
                new FileServerOptions
                {
                    FileSystem = new PhysicalFileSystem(@"./www"),
                    EnableDefaultFiles = true
                };
            builder.UseFileServer(options);

            FluentValidationModelValidatorProvider.Configure(http);
        }
    }
}