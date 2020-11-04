using System;
using System.Net.Http;
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
    }
}
