using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Interfaces;
using Cardbooru.Helpers;
using Cardbooru.Models.Base;

namespace Cardbooru.Application.Services
{
    public class ImageFetcherService : IImageFetcherService
    {
        private IBooruHttpClient _httpClient;
        private IImageCachingService _imageCachingService;
        private IBitmapImageCreatorService _bitmapImageCreatorService;
        public ImageFetcherService(
            IBooruHttpClient httpClient,
            IImageCachingService imageCachingService,
            IBitmapImageCreatorService bitmapImageCreatorService)
        {
            _httpClient = httpClient;
            _imageCachingService = imageCachingService;
            _bitmapImageCreatorService = bitmapImageCreatorService;
        }

        public async Task<BitmapImage> FetchImageAsync(
            BooruImageModelBase booruImage,
            ImageSizeType imageSizeType,
            bool caching = true,
            CancellationToken token = default)
        {
            if (!caching)
                return await _bitmapImageCreatorService.CreateImageAsync(
                    await GetImageBytes(booruImage, imageSizeType));

            if (_imageCachingService.IsHasCache(booruImage, imageSizeType))
                return await _imageCachingService.GetImageAsync(booruImage, imageSizeType, token);

            var imageBytes = await GetImageBytes(booruImage, imageSizeType);
            await _imageCachingService.CacheImageAsync(booruImage, imageSizeType, imageBytes,
                imageSizeType, token);
            return await _bitmapImageCreatorService.CreateImageAsync(imageBytes);
        }

        private async Task<byte[]> GetImageBytes(BooruImageModelBase modelBase, ImageSizeType type)
        {
            return type == ImageSizeType.Full
                ? await _httpClient.GetByteArrayAsync(modelBase.FullImageUrl)
                : await _httpClient.GetByteArrayAsync(modelBase.PreviewImageUrl);
        }
    }
}
