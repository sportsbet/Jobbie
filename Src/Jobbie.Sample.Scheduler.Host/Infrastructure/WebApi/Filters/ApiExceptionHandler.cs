using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http.Filters;
using Common.Logging;
using Jobbie.Sample.Scheduler.Contracts.Api;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Filters
{
    internal sealed class ApiExceptionHandler : ExceptionFilterAttribute
    {
        private static readonly ILog _log = LogManager.GetLogger<ApiExceptionHandler>();

        private static CustomErrorsMode ErrorMode
        {
            get
            {
                var customErrors = ConfigurationManager.GetSection("system.web/customErrors") as CustomErrorsSection;
                return customErrors?.Mode ?? CustomErrorsMode.On;
            }
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            _log.Error(context.Exception);

            var resource = new ApiError(context.Exception, ErrorMode != CustomErrorsMode.On);

            context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, resource);
        }
    }
}