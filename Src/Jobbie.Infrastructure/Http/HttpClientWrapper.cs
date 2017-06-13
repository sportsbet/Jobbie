using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Jobbie.Infrastructure.Http
{
    internal sealed class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _client;

        public HttpClientWrapper(
            HttpClient client)
        {
            _client = client;
        }

        public Uri BaseAddress
        {
            get { return _client.BaseAddress; }
            set { _client.BaseAddress = value; }
        }

        public void AddHeader(string name, string value) =>
            _client.DefaultRequestHeaders.Add(name, value);

        public void AddAuthenticationHeader(string type, string value) =>
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(type, value);

        public Task<HttpResponseMessage> GetAsync(Uri uri)
            => _client.GetAsync(uri);

        public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
            => _client.PostAsync(uri, content);

        public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content)
            => _client.PutAsync(uri, content);

        public Task<HttpResponseMessage> DeleteAsync(Uri uri)
            => _client.DeleteAsync(uri);
    }
}