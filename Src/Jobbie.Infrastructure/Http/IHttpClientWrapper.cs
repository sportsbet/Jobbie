using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Jobbie.Infrastructure.Http
{
    public interface IHttpClientWrapper
    {
        Uri BaseAddress { get; set; }
        void AddHeader(string name, string value);
        void AddAuthenticationHeader(string value, string type);
        Task<HttpResponseMessage> GetAsync(Uri uri);
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);
        Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content);
        Task<HttpResponseMessage> DeleteAsync(Uri uri);
    }
}