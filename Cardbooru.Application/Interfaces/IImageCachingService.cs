using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Entities;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;

namespace Cardbooru.Application.Interfaces
{
    public interface IImageCachingService
    {
        bool IsHasCache(IBooruPost booruImage, ImageSizeType imageType);

        Task CacheImageAsync(IBooruPost booruImage, 
            ImageSizeType imageType, byte[] bytes, 
            ImageSizeType sizeType, 
            CancellationToken cancellationToken = default);

        Task<BitmapImage> GetImageAsync(IBooruPost booruImage, 
            ImageSizeType imageType, CancellationToken cancellationToken = default);
    }
}