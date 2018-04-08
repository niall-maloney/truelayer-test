using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TrueLayer
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        public HttpClientWrapper(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        private HttpClient HttpClient { get; }

        public Uri BaseAddress
        {
            get => HttpClient.BaseAddress;
            set => HttpClient.BaseAddress = value;
        }

        public async Task<string> GetData(Uri endpoint, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint.AbsoluteUri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}