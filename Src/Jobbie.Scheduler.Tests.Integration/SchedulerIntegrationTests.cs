using System;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions;
using HoneyBear.HalClient;
using Jobbie.Sample.Scheduler.Contracts.Api;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NUnit.Framework;

namespace Jobbie.Scheduler.Tests.Integration
{
    [TestFixture, Explicit]
    public sealed class SchedulerIntegrationTests
    {
        private TestContext _context;

        [SetUp]
        public void SetUp() => _context = new TestContext();

        [Test]
        public void Authenticated_client_can_retrieve_jobs() =>
            _context
                .Arrange()
                .Act()
                .Assert();

        private sealed class TestContext
        {
            private readonly HalClient _client;
            private PagedList<Job> _result;

            private static readonly Uri _apiUrl = new Uri("http://localhost:31900/v1.0/");
            private const string Authority = "https://login.microsoftonline.com/eoin55gmail.onmicrosoft.com";
            private const string ClientId = "4aefafe1-f20e-48b7-be25-c48bb9009851";
            private const string AppKey = "jjrfaiYgeyJ/mhe44SY5Ccljxhox1xtZnReLEU4h3g0=";
            private const string ResourceId = "https://eoin55gmail.onmicrosoft.com/004f8c53-7641-4149-8b0a-b0ca7c853521";

            public TestContext()
            {
                _client = new HalClient(new HttpClient {BaseAddress = _apiUrl});
            }

            public TestContext Arrange()
            {
                var context = new AuthenticationContext(Authority);
                var credential = new ClientCredential(ClientId, AppKey);

                var result = context.AcquireTokenAsync(ResourceId, credential).Result;
                _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(result.AccessTokenType, result.AccessToken);

                return this;
            }

            public TestContext Act()
            {
                _result =
                    _client
                        .Root()
                        .Get(Relationships.Job_Query, new Page(0, 10, SortDirection.Ascending, "createdutc"), Curies.Jobbie)
                        .Item<PagedList<Job>>()
                        .Data;

                return this;
            }

            public void Assert() =>
                _result.Should().NotBeNull();
        }
    }
}