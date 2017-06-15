using System.Net.Http.Formatting;
using Jobbie.Infrastructure;
using Newtonsoft.Json.Converters;

namespace Jobbie.Sample.Client.WebApi.Host.Infrastructure.WebApi.Formatters
{
    internal sealed class MediaTypeFormatterBootstrapper : IBootstrapper
    {
        private readonly MediaTypeFormatterCollection _formatters;

        public MediaTypeFormatterBootstrapper(
            MediaTypeFormatterCollection formatters)
        {
            _formatters = formatters;
        }

        public void Init()
        {
            _formatters.Clear();

            var formatter = new JsonMediaTypeFormatter();

            formatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter());
            formatter.SerializerSettings.Converters.Add(new StringEnumConverter());

            _formatters.Add(formatter);
        }
    }
}