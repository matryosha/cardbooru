using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Interfaces;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;
using Newtonsoft.Json;
using GelbooruPost = Cardbooru.Application.Entities.GelbooruPost;
using SafebooruPost = Cardbooru.Application.Entities.SafebooruPost;

namespace Cardbooru.Application.Managers
{
    public class BooruCollectionManager : IPostCollectionManager
    {
        private IImageFetcherService _imageFetcherService;

        public BooruCollectionManager(
            IImageFetcherService imageFetcherService)
        {
            _imageFetcherService = imageFetcherService;
        }
        public List<IBooruPost> DeserializePosts(BooruSiteType type, string posts)
        {
            switch (type)
            {
                case BooruSiteType.Danbooru:
                    return new List<IBooruPost>(
                        JsonConvert.DeserializeObject<List<DanbooruPost>>(posts));
                case BooruSiteType.SafeBooru:
                    return new List<IBooruPost>(
                        JsonConvert.DeserializeObject<List<SafebooruPost>>(posts));
                case BooruSiteType.Gelbooru:
                    return new List<IBooruPost>(
                        JsonConvert.DeserializeObject<List<GelbooruPost>>(posts));
            }

            throw new Exception("Unknown booru type for deserialize");
        }

        public async Task<List<BooruImageWrapper>> GetImagesAsync(
            BooruSiteType booruSiteType,
            ImageSizeType imageType,
            ICollection<IBooruPost> collection,
            CancellationToken cancellationToken = default)
        {
            var images = new List<BooruImageWrapper>();

            foreach (var booruImage in collection)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                //Todo research why hash may be empty
                if (string.IsNullOrEmpty(booruImage.Hash)) continue;

                var imageFile = await _imageFetcherService.FetchImageAsync(
                    booruImage, imageType, cancellationToken: cancellationToken);

                if (imageFile == null) continue;
                var booruImageWrapper  = new BooruImageWrapper();
                booruImageWrapper.Hash = booruImage.Hash;
                booruImageWrapper.Image = imageFile;
                images.Add(booruImageWrapper);
            }

            return images;
        }

    }
}