using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Helpers;
using Cardbooru.Models.Base;

namespace Cardbooru.Application.Interfaces
{
    public interface IImageCachingService
    {
        bool IsHasCache(BooruImageModelBase booruImage, ImageSizeType imageType);

        Task CacheImageAsync(BooruImageModelBase booruImage, 
            ImageSizeType imageType, byte[] bytes, 
            ImageSizeType sizeType, 
            CancellationToken cancellationToken = default);

        Task<BitmapImage> GetImageAsync(BooruImageModelBase booruImage, 
            ImageSizeType imageType, CancellationToken cancellationToken = default);
    }
}