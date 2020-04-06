using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Helpers;
using Cardbooru.Application.Interfaces;
using Cardbooru.Core;

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
            FetchConfiguration configuration)
        {
            _httpClient = httpClient;
            _helper = helper;
            _initLimit = configuration.PostLimit;
        }

        public Task<string> FetchPostsAsync(
            BooruSiteType type, 
            int limit = 0, 
            int pageNumber = 1, 
            ICollection<string> tags = null,
            CancellationToken cancellationToken = default)
        {
            if (limit == 0)
                limit = _initLimit;

            var url = _helper.GetPostsUrl(
                type, limit, pageNumber, tags);

            return _httpClient.GetStringAsync(url, cancellationToken);
        }
    }
}