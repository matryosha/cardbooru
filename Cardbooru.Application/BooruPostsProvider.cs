﻿using System;
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

        public int QueryPage { get; set; }
        public BooruSiteType SiteType { get; set; }
        public int QueryPostLimit { get; set; }
        public List<string> Tags { get; set; }
        public List<IBooruPost> Posts { get; set; }
        public List<BooruImageWrapper> BooruPreviewImages { get; set; }

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
            if(BooruPreviewImages == null)
                BooruPreviewImages = new List<BooruImageWrapper>();

            if(Posts == null)
                Posts = new List<IBooruPost>();

            QueryPage = queryPageNumber;
            SiteType = siteType;
            QueryPostLimit = postLimit;

            var ratingTags = _postManager.GetRatingTagString(_configuration.ActiveSite,
                _configuration.FetchConfiguration.RatingConfiguration);
            Tags = new List<string> {ratingTags};
            Tags.AddRange(tags);
            BooruPreviewImages.Clear();
            Posts.Clear();

            var postsString = 
                await _postFetcherService.FetchPostsAsync(
                    SiteType,
                    QueryPostLimit, 
                    tags: Tags, 
                    pageNumber: QueryPage, 
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
                Tags,
                ++QueryPage,
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

        public BooruImageWrapper GetPreviewBooruImage(IBooruPost post)
        {
            return BooruPreviewImages.FirstOrDefault(i => i.Hash == post.Hash);
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

            BooruPreviewImages.Add(booruImage);
        }
        
    }
}