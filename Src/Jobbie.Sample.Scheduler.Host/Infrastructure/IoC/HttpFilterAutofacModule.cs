using System.Web.Http.Filters;
using Autofac;
using Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Filters;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.IoC
{
    internal sealed class HttpFilterAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<ApiExceptionHandler>()
                .AsSelf();

            builder
                .RegisterType<WebApiAuthorizeAttribute>()
                .As<IAuthorizationFilter>();
        }
    }
}