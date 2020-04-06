using Cardbooru.Application.Interfaces;
using Cardbooru.Core;
using Cardbooru.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Cardbooru.Application.Services
{
    public class ImageFetcherService : IImageFetcherService
    {
        private readonly IBooruHttpClient _httpClient;
        private readonly IImageCachingService _imageCachingService;
        private readonly IBooruConfiguration _configuration;

        public ImageFetcherService(
            IBooruHttpClient httpClient,
            IImageCachingService imageCachingService,
            IBooruConfiguration configuration)
        {
            _httpClient = httpClient;
            _imageCachingService = imageCachingService;
            _configuration = configuration;
        }

        public async Task<byte[]> FetchImageAsync(
            IBooruPost booruPost,
            ImageSizeType imageSizeType,
            bool caching = true,
            CancellationToken cancellationToken = default)
        {
            caching = _configuration.ImageCaching;
            cancellationToken.ThrowIfCancellationRequested();
            byte[] resultBytes;
            if (caching)
            {
                if (_imageCachingService.IsHasCache(booruPost, imageSizeType))
                {
                    resultBytes = await _imageCachingService.GetImageAsync(
                        booruPost, imageSizeType, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    resultBytes = await GetImageBytes(
                        booruPost, imageSizeType, cancellationToken).ConfigureAwait(false);
                    await _imageCachingService.CacheImageAsync(booruPost, imageSizeType, resultBytes,
                        imageSizeType, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                resultBytes = await GetImageBytes(booruPost, imageSizeType, cancellationToken).ConfigureAwait(false);
            }


            return resultBytes;
        }

        private async Task<byte[]> GetImageBytes(IBooruPost modelBase,
            ImageSizeType type,
            CancellationToken cancellationToken)
        {
            return type == ImageSizeType.Full
                ? await _httpClient.GetByteArrayAsync(modelBase.FullImageUrl, cancellationToken).ConfigureAwait(false)
                : await _httpClient.GetByteArrayAsync(modelBase.PreviewImageUrl, cancellationToken).ConfigureAwait(false);
        }
    }
}
