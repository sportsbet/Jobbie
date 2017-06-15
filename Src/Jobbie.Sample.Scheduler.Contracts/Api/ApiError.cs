using System;
using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Contracts.Api
{
    public sealed class ApiError : Representation
    {
        public ApiError(Exception exception, bool includeFullDetails)
        {
            Error = exception.GetType().Name;
            Message = exception.Message;
            if (includeFullDetails)
                Exception = exception.ToString();
        }

        public ApiError(Exception exception)
            : this(exception, false)
        {

        }

        public ApiError()
        {

        }

        public string Error { get; }

        public string Message { get; }

        public string Exception { get; }
    }
}