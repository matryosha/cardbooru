using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Interfaces;
using Cardbooru.Helpers;
using Cardbooru.Models;
using Cardbooru.Models.Base;
using Newtonsoft.Json;

namespace Cardbooru.Application
{
    public class BooruCollectionManager : IPostCollectionManager
    {
        private IImageFetcherService _imageFetcherService;

        public BooruCollectionManager(
            IImageFetcherService imageFetcherService)
        {
            _imageFetcherService = imageFetcherService;
        }
        public List<BooruImageModelBase> DeserializePosts(BooruSiteType type, string posts)
        {
            switch (type)
            {
                case BooruSiteType.Danbooru:
                    return new List<BooruImageModelBase>(
                        JsonConvert.DeserializeObject<List<DanbooruImageModel>>(posts));
                case BooruSiteType.SafeBooru:
                    return new List<BooruImageModelBase>(
                        JsonConvert.DeserializeObject<List<SafebooruImageModel>>(posts));
                case BooruSiteType.Gelbooru:
                    return new List<BooruImageModelBase>(
                        JsonConvert.DeserializeObject<List<GelbooruImageModel>>(posts));
            }

            throw new Exception("Unknown booru type for deserialize");
        }

        public async Task<List<BooruImageWrapper>> GetImagesAsync(
            BooruSiteType booruSiteType,
            ImageSizeType imageType,
            ICollection<BooruImageModelBase> collection,
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