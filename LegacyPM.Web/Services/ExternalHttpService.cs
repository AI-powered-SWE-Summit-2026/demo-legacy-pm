using System.Net.Http;
using System.Threading.Tasks;

namespace LegacyPM.Web.Services
{
    public class ExternalHttpService
    {
        public async Task<string> GetExternalData(string url)
        {
            using var client = new HttpClient();
            return await client.GetStringAsync(url);
        }

        public string GetExternalDataBlocking(string url)
        {
            using var client = new HttpClient();
            return client.GetStringAsync(url).Result;
        }
    }
}