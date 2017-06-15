using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Jobbie.Infrastructure;

namespace Jobbie.Sample.Client.WebApi.Host.Infrastructure.WebApi.HttpRoutes
{
    internal sealed class HttpRouteBootstrapper : IBootstrapper
    {
        private readonly HttpRouteCollection _routes;

        public HttpRouteBootstrapper(
            HttpRouteCollection routes)
        {
            _routes = routes;
        }

        public void Init()
        {
            _routes
                .MapHttpRoute(
                    "CreateResource",
                    "{controller}",
                    new {action = "Create"},
                    new
                    {
                        httpMethod = new HttpMethodConstraint(new HttpMethod("Post"))
                    });

            _routes
                .MapHttpRoute(
                    "GetResources",
                    "{controller}",
                    new {action = "Get"},
                    new
                    {
                        httpMethod = new HttpMethodConstraint(new HttpMethod("Get"))
                    });
        }
    }
}