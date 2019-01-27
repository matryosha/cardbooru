using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Interfaces;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;

namespace Cardbooru.Application
{
    public class BooruPostsProvider
    {
        private readonly IPostFetcherService _postFetcherService;
        private readonly IBooruPostManager _postManager;
        private readonly IImageFetcherService _imageFetcherService;
        private readonly IBooruConfiguration _configuration;
        private int _queryPage;
        private BooruSiteType _siteType;
        private int _queryPostLimit;
        private List<string> _tags;

        public List<IBooruPost> Posts { get; private set; }
        public List<BooruImageWrapper> BooruImages { get; private set; }

        public BooruPostsProvider(
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addImageCallback">Invoke when adding <see cref="BooruImageWrapper"/></param>
        public async Task GetPosts(
            BooruSiteType siteType, 
            Action<BooruImageWrapper> addImageCallback,
            IList<string> tags,
            int queryPageNumber,
            int postLimit,
            CancellationToken cancellationToken = default
            )
        {
            if(BooruImages == null)
                BooruImages = new List<BooruImageWrapper>();

            if(Posts == null)
                Posts = new List<IBooruPost>();

            _queryPage = queryPageNumber;
            _siteType = siteType;
            _queryPostLimit = postLimit;

            var ratingTags = _postManager.GetRatingTagString(_configuration.ActiveSite,
                _configuration.FetchConfiguration.RatingConfiguration);
            _tags = new List<string> {ratingTags};
            _tags.AddRange(tags);
            BooruImages.Clear();
            Posts.Clear();

            var postsString = 
                await _postFetcherService.FetchPostsAsync(
                    _siteType,
                    _queryPostLimit, 
                    tags: _tags, 
                    pageNumber: _queryPage, 
                    cancellationToken: cancellationToken).ConfigureAwait(false);

            var posts = _postManager.DeserializePosts(
                _configuration.ActiveSite, postsString);

            foreach (var post in posts)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Posts.Add(post);
                await AddBooruImage(post, addImageCallback, cancellationToken);
            }

        }

        public async Task GetPosts(Action<BooruImageWrapper> addImageCallback,
            int queryPage,
            CancellationToken cancellationToken = default)
        {
            await GetPosts(
                _configuration.ActiveSite,
                addImageCallback,
                new List<string>(),
                queryPage,
                _configuration.FetchConfiguration.PostLimit,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task GetNextPosts(Action<BooruImageWrapper> addImageCallback,
            CancellationToken cancellationToken = default)
        {
            await GetPosts(
                _configuration.ActiveSite,
                addImageCallback,
                _tags,
                ++_queryPage,
                _configuration.FetchConfiguration.PostLimit,
                cancellationToken).ConfigureAwait(false);
        }

        public IBooruPost GetBooruPost(BooruImageWrapper booruImage)
        {
            return Posts.FirstOrDefault(p => p.Hash == booruImage.Hash);
        }

        public IBooruPost GetBooruPost(string booruImageHash)
        {
            return Posts.FirstOrDefault(p => p.Hash == booruImageHash);
        }

        public List<IBooruPost> GetCurrentBooruPosts()
        {
            return new List<IBooruPost>(Posts);
        }

        private async Task AddBooruImage(IBooruPost post,
            Action<BooruImageWrapper> addImageCallback,
            CancellationToken cancellationToken)
        {
            var image =
                await _imageFetcherService.FetchImageAsync(
                post,
                ImageSizeType.Preview,
                caching: false,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            var booruImage = new BooruImageWrapper
            {
                Hash = post.Hash,
                Image = image
            };

            addImageCallback.Invoke(booruImage);

            BooruImages.Add(booruImage);
        }
        
    }
}