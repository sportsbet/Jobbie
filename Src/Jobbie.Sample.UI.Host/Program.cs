using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Jobbie.Sample.UI.Host
{
    internal sealed class Program
    {
        public static void Main() =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://*:31902")
                .UseIISIntegration()
                .Build()
                .Run();
    }
}
