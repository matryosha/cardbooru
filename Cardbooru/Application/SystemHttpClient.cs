using System.Net.Http;
using System.Threading.Tasks;
using Cardbooru.Application.Interfaces;

namespace Cardbooru.Application
{
    public class SystemHttpClient : IBooruHttpClient
    {
        private HttpClient _client;

        public SystemHttpClient()
        {
            _client = new HttpClient();
        }
        public Task<string> GetStringAsync(string url)
        {
            return _client.GetStringAsync(url);
        }

        public Task<byte[]> GetByteArrayAsync(string url)
        {
            return _client.GetByteArrayAsync(url);
        }
    }
}
