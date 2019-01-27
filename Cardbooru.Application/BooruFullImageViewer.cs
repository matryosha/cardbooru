using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Exceptions;
using Cardbooru.Application.Interfaces;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;

namespace Cardbooru.Application
{
    public class BooruFullImageViewer
    {
        private readonly IImageFetcherService _imageFetcherService;
        private readonly IBooruConfiguration _configuration;
        private readonly IBooruPostsProviderFactory _postsProviderFactory;
        private BooruPostsProvider _postsProvider;
        private BooruImageWrapper _currentBooruImage;
        private IBooruPost _currentBooruPost;
        private int _currentBooruImageIndex = -1;

        public void Init(BooruPostsProvider provider,
            BooruImageWrapper currentBooruImage)
        {
            _postsProvider = _postsProviderFactory.CreateFrom(provider);
            _currentBooruImage = currentBooruImage;
        }

        public BooruFullImageViewer(
            IImageFetcherService imageFetcherService,
            IBooruConfiguration configuration,
            IBooruPostsProviderFactory postsProviderFactory)
        {
            _imageFetcherService = imageFetcherService;
            _configuration = configuration;
            _postsProviderFactory = postsProviderFactory;
        }

        public async Task<BooruImageWrapper> GetNextBooruImageAsync(
            Action<BooruImageWrapper> previewImageLoadedCallback,
            CancellationToken cancellationToken)
        {
            if (_currentBooruImageIndex == -1)
            {
                _currentBooruImageIndex = GetBooruImageIndex(_currentBooruImage);
            }

            if (_currentBooruImageIndex == _postsProvider.BooruPreviewImages.Count - 1)
            {
                var nextProvider = _postsProviderFactory.CreateFrom(_postsProvider);

                await nextProvider.GetNextPosts(wrapper => { }, cancellationToken);

                _currentBooruImageIndex = -1;
                _postsProvider = nextProvider;

            }

            return await GetBooruImageAsync(++_currentBooruImageIndex,
                previewImageLoadedCallback,
                cancellationToken);
        }

        public async Task<BooruImageWrapper> GetPrevBooruImageAsync(
            Action<BooruImageWrapper> previewImageLoadedCallback,
            CancellationToken cancellationToken)
        {
            if (_currentBooruImageIndex == -1)
            {
                _currentBooruImageIndex = GetBooruImageIndex(_currentBooruImage);
            }

            if (_currentBooruImageIndex == 0)
            {
                if (_postsProvider.QueryPage == 1)
                {
                    throw new QueryPageException("First page");
                }

                var nextProvider = _postsProviderFactory.CreateFrom(_postsProvider);
                await nextProvider.GetPrevPosts(wrapper => { }, cancellationToken);

                _currentBooruImageIndex = nextProvider.BooruPreviewImages.Count;
                _postsProvider = nextProvider;
            }

            return await GetBooruImageAsync(--_currentBooruImageIndex, 
                previewImageLoadedCallback,
                cancellationToken);
        }

        public Task<BitmapImage> FetchImageAsync(BooruImageWrapper booruImage,
            CancellationToken cancellationToken = default)
        {
            return FetchImageAsync(
                _postsProvider.GetBooruPost(booruImage), 
                cancellationToken);  
        }

        public Task<BitmapImage> FetchImageAsync(string booruImageHash,
            CancellationToken cancellationToken = default)
        {
            return FetchImageAsync(
                _postsProvider.GetBooruPost(booruImageHash),
                cancellationToken);
        }

        public Task<BitmapImage> FetchImageAsync(IBooruPost post,
            CancellationToken cancellationToken)
        {
            return _imageFetcherService.FetchImageAsync(
                post,
                ImageSizeType.Full,
                _configuration.ImageCaching,
                cancellationToken);
        }

        private async Task<BooruImageWrapper> GetBooruImageAsync(int booruImageIndex,
            Action<BooruImageWrapper> previewImageLoadedCallback,
            CancellationToken cancellationToken)
        {
            var previewBooruImage = 
                _postsProvider.BooruPreviewImages[booruImageIndex];
            previewImageLoadedCallback.Invoke(previewBooruImage);

            var booruImageHash = previewBooruImage.Hash;
            var booruPost = _postsProvider.Posts.FirstOrDefault(p => p.Hash == booruImageHash);
            BitmapImage fullImage = null;

            fullImage = await _imageFetcherService.FetchImageAsync(booruPost, ImageSizeType.Full,
                cancellationToken: cancellationToken);

            return new BooruImageWrapper
            {
                Hash = booruImageHash,
                Image = fullImage
            };
        }

        private int GetBooruImageIndex(BooruImageWrapper booruImage)
        {
            return _postsProvider.BooruPreviewImages.IndexOf(booruImage);
        }

        private int GetBooruImageIndex(IBooruPost post)
        {
            return _postsProvider.BooruPreviewImages.IndexOf(
                _postsProvider.GetPreviewBooruImage(post));
        }
    }
}