using Cardbooru.Application.Interfaces;

namespace Cardbooru.Application.Infrastructure
{
    public class DefaultBooruPostsProviderFactory : IBooruPostsProviderFactory
    {
        private readonly IPostFetcherService _postFetcherService;
        private readonly IBooruPostManager _postManager;
        private readonly IImageFetcherService _imageFetcherService;
        private readonly IBooruConfiguration _configuration;

        public DefaultBooruPostsProviderFactory(
            IPostFetcherService postFetcherService,
            IBooruPostManager postManager,
            IImageFetcherService imageFetcherService,
            IBooruConfiguration configuration)
        {
            _postFetcherService = postFetcherService;
            _postManager = postManager;
            _imageFetcherService = imageFetcherService;
            _configuration = configuration;
        }

        public BooruPostsProvider Create()
        {
            return new BooruPostsProvider(_postFetcherService,_postManager,_imageFetcherService,_configuration);
        }
    }
}