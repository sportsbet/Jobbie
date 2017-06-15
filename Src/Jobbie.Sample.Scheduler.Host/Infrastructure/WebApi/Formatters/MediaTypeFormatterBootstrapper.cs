using System.Net.Http.Formatting;
using Jobbie.Infrastructure;
using Jobbie.Sample.Scheduler.Host.Infrastructure.Hypermedia;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using WebApi.Hal;

namespace Jobbie.Sample.Scheduler.Host.Infrastructure.WebApi.Formatters
{
    internal sealed class MediaTypeFormatterBootstrapper : IBootstrapper
    {
        private readonly MediaTypeFormatterCollection _formatters;
        private readonly IHypermediaConfiguration _hypermedia;

        public MediaTypeFormatterBootstrapper(
            MediaTypeFormatterCollection formatters,
            IHypermediaConfiguration hypermedia)
        {
            _formatters = formatters;
            _hypermedia = hypermedia;
        }

        public void Init()
        {
            _formatters.Clear();

            var formatter =
                new JsonHalMediaTypeFormatter(_hypermedia.Resolver)
                {
                    SerializerSettings =
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
#if DEBUG
                        Formatting = Formatting.Indented
#endif
                    }
                };

            formatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            formatter.SerializerSettings.Converters.Add(new StringEnumConverter());

            _formatters.Add(formatter);

            _formatters.Add(new JsonMediaTypeFormatter());
        }
    }
}