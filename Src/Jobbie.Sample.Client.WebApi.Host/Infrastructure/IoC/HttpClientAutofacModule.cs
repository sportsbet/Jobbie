using System;
using System.Net.Http;
using Autofac;
using HoneyBear.HalClient;

namespace Jobbie.Sample.Client.WebApi.Host.Infrastructure.IoC
{
    internal sealed class HttpClientAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<HttpClient>()
                .WithProperty("BaseAddress", new Uri("http://localhost:31900/v1.0"))
                .AsSelf();

            builder
                .RegisterType<HalClient>()
                .AsImplementedInterfaces();
        }
    }
}