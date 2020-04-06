using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private BooruImage _currentBooruImage;
        private IBooruPost _currentBooruPost;
        private int _currentBooruImageIndex = -1;

        public IBooruPost CurrentBooruPost => 
            _currentBooruPost ?? (
                _currentBooruPost = 
                    _postsProvider.GetBooruPost(CurrentBooruImage));

        public BooruImage CurrentBooruImage
        {
            get => _currentBooruImage;
            set
            {
                _currentBooruPost = null;
                _currentBooruImage = value;
            }
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

        public void Init(BooruPostsProvider provider,
            BooruImage currentBooruImage)
        {
            _postsProvider = _postsProviderFactory.CreateFrom(provider);
            CurrentBooruImage = currentBooruImage;
        }

        public async Task<BooruImage> GetNextBooruImageAsync(
            Action<BooruImage> previewImageLoadedCallback,
            CancellationToken cancellationToken)
        {
            if (_currentBooruImageIndex == -1)
            {
                _currentBooruImageIndex = GetBooruImageIndex(CurrentBooruImage);
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

        public async Task<BooruImage> GetPrevBooruImageAsync(
            Action<BooruImage> previewImageLoadedCallback,
            CancellationToken cancellationToken)
        {
            if (_currentBooruImageIndex == -1)
            {
                _currentBooruImageIndex = GetBooruImageIndex(CurrentBooruImage);
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

        public Task<byte[]> FetchImageAsync(BooruImage booruImage,
            CancellationToken cancellationToken = default)
        {
            return FetchImageAsync(
                _postsProvider.GetBooruPost(booruImage), 
                cancellationToken);  
        }

        public Task<byte[]> FetchImageAsync(string booruImageHash,
            CancellationToken cancellationToken = default)
        {
            return FetchImageAsync(
                _postsProvider.GetBooruPost(booruImageHash),
                cancellationToken);
        }

        public Task<byte[]> FetchImageAsync(IBooruPost post,
            CancellationToken cancellationToken)
        {
            return _imageFetcherService.FetchImageAsync(
                post,
                ImageSizeType.Full,
                _configuration.ImageCaching,
                cancellationToken);
        }

        public List<string> GetTags()
        {
            return CurrentBooruPost.TagsString.Split(' ').ToList();
        }

        private async Task<BooruImage> GetBooruImageAsync(int booruImageIndex,
            Action<BooruImage> previewImageLoadedCallback,
            CancellationToken cancellationToken)
        {
            var previewBooruImage = 
                _postsProvider.BooruPreviewImages[booruImageIndex];
            previewImageLoadedCallback.Invoke(previewBooruImage);

            var booruImageHash = previewBooruImage.Hash;
            var booruPost = _postsProvider.Posts.FirstOrDefault(p => p.Hash == booruImageHash);
            byte[] fullImage;

            fullImage = await _imageFetcherService.FetchImageAsync(booruPost, ImageSizeType.Full,
                cancellationToken: cancellationToken);

            CurrentBooruImage = new BooruImage
            {
                Hash = booruImageHash,
                Data = fullImage
            };

            return CurrentBooruImage;
        }

        private int GetBooruImageIndex(BooruImage booruImage)
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