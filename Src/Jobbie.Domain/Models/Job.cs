using System;
using System.Collections.Generic;
using System.Linq;

namespace Jobbie.Domain.Models
{
    public sealed class Job
    {
        public Job(
            Guid jobId,
            string description,
            Uri callbackUrl,
            HttpVerb httpVerb,
            string payload,
            string contentType,
            string headers,
            DateTime createdUtc,
            bool isOnceOff,
            TimeSpan? timeout)
        {
            JobId = jobId;
            Description = description;
            CallbackUrl = callbackUrl;
            HttpVerb = httpVerb;
            Payload = payload;
            ContentType = contentType;
            Headers = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(headers))
                foreach (var h in headers.Split(';').Select(x => x.Split(':', '=')))
                    Headers.Add(h[0], h[1]);
            CreatedUtc = createdUtc;
            IsOnceOff = isOnceOff;
            Timeout = timeout;
        }

        public Job(
            Guid jobId,
            string description,
            Uri callbackUrl,
            HttpVerb httpVerb,
            string payload,
            string contentType,
            string headers,
            DateTime createdUtc,
            bool isOnceOff)
            : this(jobId, description, callbackUrl, httpVerb, payload, contentType, headers, createdUtc, isOnceOff, null)
        {
            
        }

        public Guid JobId { get; }

        public string Description { get; }

        public Uri CallbackUrl { get; }

        public HttpVerb HttpVerb { get; }

        public string Payload { get; }

        public string ContentType { get; }

        public IDictionary<string, string> Headers { get; }

        public DateTime CreatedUtc { get; }

        public bool IsOnceOff { get; }

        public TimeSpan? Timeout { get; }

        public override string ToString() => $"JobId={JobId}|Description={Description}";
    }
}