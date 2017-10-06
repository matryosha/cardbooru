using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cardbooru
{
     public class Model {
        private const int DefaultLimitForRequest = 10;
        private const string Danbooru = "https://danbooru.donmai.us";
        private HttpClient _client;

        public List<BooruImage> BooruImagesList { get; set; }

        public async Task<string> GetImages(int pageNum) {
            if(BooruImagesList == null)
                BooruImagesList = new List<BooruImage>();

            var posts = await GetClient()
                .GetStringAsync(Danbooru + $"/posts.json?limit={DefaultLimitForRequest}&page={pageNum}");

            BooruImagesList = JsonConvert.DeserializeObject<List<BooruImage>>(posts);

            return "okay";
        }

        private HttpClient GetClient() {
            if (_client == null) {
                return new HttpClient();
            }
            return _client;
        }
    }
}
