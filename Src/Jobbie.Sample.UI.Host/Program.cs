using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Jobbie.Sample.UI.Host
{
    internal sealed class Program
    {
        private static void Main() =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://*:31902")
                .Build()
                .Run();
    }
}
