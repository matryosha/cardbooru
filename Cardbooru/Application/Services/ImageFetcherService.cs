using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Interfaces;
using Cardbooru.Helpers;
using Cardbooru.Models.Base;

namespace Cardbooru.Application.Services
{
    public class ImageFetcherService : IImageFetcherService
    {
        private readonly IBooruHttpClient _httpClient;
        private readonly IImageCachingService _imageCachingService;
        private readonly IBitmapImageCreatorService _bitmapImageCreatorService;
        private readonly RootConfiguration _configuration;

        public ImageFetcherService(
            IBooruHttpClient httpClient,
            IImageCachingService imageCachingService,
            IBitmapImageCreatorService bitmapImageCreatorService,
            RootConfiguration configuration)
        {
            _httpClient = httpClient;
            _imageCachingService = imageCachingService;
            _bitmapImageCreatorService = bitmapImageCreatorService;
            _configuration = configuration;
        }

        public async Task<BitmapImage> FetchImageAsync(
            BooruImageModelBase booruImage,
            ImageSizeType imageSizeType,
            bool caching = true,
            CancellationToken cancellationToken = default)
        {
            caching = _configuration.ImageCaching;
            cancellationToken.ThrowIfCancellationRequested();
            BitmapImage resultImage;
            if (caching)
            {
                if (_imageCachingService.IsHasCache(booruImage, imageSizeType))
                {
                    resultImage = await _imageCachingService.GetImageAsync(
                        booruImage, imageSizeType, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var imageBytes = await GetImageBytes(
                        booruImage, imageSizeType, cancellationToken).ConfigureAwait(false);
                    await _imageCachingService.CacheImageAsync(booruImage, imageSizeType, imageBytes,
                        imageSizeType, cancellationToken).ConfigureAwait(false);
                    resultImage = 
                        await _bitmapImageCreatorService.CreateImageAsync(imageBytes).ConfigureAwait(false);
                }
            }
            else
            {
                resultImage = 
                    await  _bitmapImageCreatorService.CreateImageAsync(await GetImageBytes(
                        booruImage, imageSizeType, cancellationToken).ConfigureAwait(false))
                    .ConfigureAwait(false);
            }

            
            return resultImage;
            //    return await _bitmapImageCreatorService.CreateImageAsync(
            //        await GetImageBytes(booruImage, imageSizeType));

            //if (_imageCachingService.IsHasCache(booruImage, imageSizeType))
            //    return await _imageCachingService.GetImageAsync(booruImage, imageSizeType, token);

            //var imageBytes = await GetImageBytes(booruImage, imageSizeType);
            //await _imageCachingService.CacheImageAsync(booruImage, imageSizeType, imageBytes,
            //    imageSizeType, token);
            //return await _bitmapImageCreatorService.CreateImageAsync(imageBytes);
        }

        private async Task<byte[]> GetImageBytes(BooruImageModelBase modelBase, 
            ImageSizeType type,
            CancellationToken cancellationToken)
        {
            return type == ImageSizeType.Full
                ? await _httpClient.GetByteArrayAsync(modelBase.FullImageUrl, cancellationToken).ConfigureAwait(false)
                : await _httpClient.GetByteArrayAsync(modelBase.PreviewImageUrl, cancellationToken).ConfigureAwait(false);
        }
    }
}
