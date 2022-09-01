using Minsky.Entities.Integration.Sneaker;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Minsky.Integaration
{
    public sealed class SneakerApiClient
    {
        public async Task<ServerInfoContract> GetServerInfoAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("User-Agent", "Minsky");

                var byteResult = await client.GetStreamAsync("http://176.114.3.199:7788/api/servers");
                
                var result = (await JsonSerializer.DeserializeAsync<List<ServerInfoContract>>(byteResult)).SingleOrDefault();
                
                return result;
            }
        }
    }
}
