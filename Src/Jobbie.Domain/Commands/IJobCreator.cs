using System;

namespace Jobbie.Domain.Commands
{
    public interface IJobCreator
    {
        void Create(
            Guid jobId,
            string description,
            string callbackUrl,
            string httpVerb,
            string payload,
            string contentType,
            string headers);

        void Create(
            Guid jobId,
            string description,
            string callbackUrl,
            string httpVerb,
            string payload,
            string contentType,
            string headers,
            TimeSpan? timeout);
    }
}