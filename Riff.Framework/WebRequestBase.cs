using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Framework
{
    public class WebRequest
    {
        public async Task<string> GetRequest(string path)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(path)
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                var responseInfo = await response.Content.ReadAsStringAsync();
                return responseInfo;
            }
        }

        public async Task<string> PostRequest(string path, string authHeader, object requestBody, bool bearerAuth = true)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(path)
                };
                var requestBodyPresent = requestBody != null;
                if (requestBodyPresent)
                {
                    var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                    request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
                }
                if (bearerAuth)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authHeader);
                }
                var response = await client.SendAsync(request).ConfigureAwait(false);
                var responseInfo = await response.Content.ReadAsStringAsync();
                return responseInfo;
            }
        }
    }
}
