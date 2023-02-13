using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Framework
{
    class customObj
    {
        public string param1 { get; set; }
        public string param2 { get; set; }
        public string param3 { get; set; }
    }

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

        public async Task<string> PostRequest(string path, string authHeader, bool bearerAuth = true)
        {
            using (var client = new HttpClient())
            {
                customObj custom = new customObj();
                var jsonContent = JsonConvert.SerializeObject(custom);
               
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(path),
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                };
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
