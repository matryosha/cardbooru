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

        public BooruPostsProvider CreateFrom(BooruPostsProvider srcProvider)
        {
            var copyProvider = new BooruPostsProvider(_postFetcherService, _postManager, _imageFetcherService, _configuration);
            var t = typeof(BooruPostsProvider);
            foreach (var f in t.GetProperties())
            {
                var dstF = t.GetProperty(f.Name);
                if (dstF == null)
                    continue;

                dstF.SetValue(copyProvider, f.GetValue(srcProvider, null), null);
            }

            return copyProvider;
        }
    }
}