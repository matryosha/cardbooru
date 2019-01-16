using System.Collections.Generic;
using System.Threading.Tasks;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Helpers;
using Cardbooru.Application.Interfaces;
using Cardbooru.Helpers;

namespace Cardbooru.Application.Services
{
    public class PostFetcherService : IPostFetcherService
    {
        private IBooruHttpClient _httpClient;
        private PostFetcherServiceHelper _helper;
        private int _initLimit;

        public PostFetcherService(
            IBooruHttpClient httpClient,
            PostFetcherServiceHelper helper,
            UrlConfiguration configuration)
        {
            _httpClient = httpClient;
            _helper = helper;
            _initLimit = configuration.PostLimit;
        }

        public Task<string> FetchPostsAsync(
            BooruSiteType type, 
            int limit = 0, 
            int pageNumber = 1, 
            ICollection<string> tags = null)
        {
            if (limit == 0)
                limit = _initLimit;

            var url = _helper.GetPostsUrl(
                type, limit, pageNumber, tags);

            return _httpClient.GetStringAsync(url);
        }
    }
}