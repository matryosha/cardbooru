using Cardbooru.Application.Interfaces;

namespace Cardbooru.Application.Infrastructure
{
    public class DefaultBooruFullImageViewerFactory : IBooruFullImageViewerFactory
    {
        private readonly IImageFetcherService _imageFetcherService;
        private readonly IBooruConfiguration _configuration;
        private readonly IBooruPostsProviderFactory _postsProviderFactory;

        public DefaultBooruFullImageViewerFactory(
            IImageFetcherService imageFetcherService,
            IBooruConfiguration configuration,
            IBooruPostsProviderFactory postsProviderFactory)
        {
            _imageFetcherService = imageFetcherService;
            _configuration = configuration;
            _postsProviderFactory = postsProviderFactory;
        }
        public BooruFullImageViewer Create()
        {
            return new BooruFullImageViewer(
                _imageFetcherService,
                _configuration,
                _postsProviderFactory);
        }
    }
}