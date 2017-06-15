using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Jobbie.Infrastructure;
using Jobbie.Sample.Scheduler.Host.Infrastructure.Versioning;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.HttpRoutes
{
    internal sealed class HttpRouteBootstrapper : IBootstrapper
    {
        private readonly HttpRouteCollection _routes;
        private readonly ApiVersionRouteConstraint _versionConstraint;
        private readonly ILatestApiVersion _version;

        public HttpRouteBootstrapper(
            HttpRouteCollection routes,
            ApiVersionRouteConstraint versionConstraint,
            ILatestApiVersion version)
        {
            _routes = routes;
            _versionConstraint = versionConstraint;
            _version = version;
        }

        public void Init()
        {
            _routes
                .MapHttpRoute(
                    "DeleteResource",
                    "v{apiVersion}/{controller}/{id}",
                    new {action = "Delete"},
                    new
                    {
                        httpMethod = new HttpMethodConstraint(new HttpMethod("Delete")),
                        apiVersion = _versionConstraint
                    });

            _routes
                .MapHttpRoute(
                    "UpdateResource",
                    "v{apiVersion}/{controller}/{id}",
                    new {action = "Update"},
                    new
                    {
                        httpMethod = new HttpMethodConstraint(new HttpMethod("Put")),
                        apiVersion = _versionConstraint
                    });

            _routes
                .MapHttpRoute(
                    "CreateResource",
                    "v{apiVersion}/{controller}",
                    new {action = "Create"},
                    new
                    {
                        httpMethod = new HttpMethodConstraint(new HttpMethod("Post")),
                        apiVersion = _versionConstraint
                    });

            _routes
                .MapHttpRoute(
                    "GetResource",
                    "v{apiVersion}/{controller}/{id}",
                    new {action = "Get"},
                    new
                    {
                        httpMethod = new HttpMethodConstraint(new HttpMethod("Get")),
                        apiVersion = _versionConstraint
                    });

            _routes
                .MapHttpRoute(
                    "GetResources",
                    "v{apiVersion}/{controller}",
                    new {action = "Get"},
                    new
                    {
                        httpMethod = new HttpMethodConstraint(new HttpMethod("Get")),
                        apiVersion = _versionConstraint
                    });

            _routes
                .MapHttpRoute(
                    "GetVersionDefault",
                    "v{apiVersion}",
                    new {action = "Get", controller = "ApiVersion", id = _version},
                    new
                    {
                        httpMethod = new HttpMethodConstraint(new HttpMethod("Get")),
                        apiVersion = _versionConstraint
                    });

            _routes
                .MapHttpRoute(
                    "GetDefault",
                    "",
                    new {action = "Get", controller = "ApiVersion", apiVersion = _version},
                    new
                    {
                        httpMethod = new HttpMethodConstraint(new HttpMethod("Get"))
                    });
        }
    }
}