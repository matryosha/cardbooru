using System.Threading;
using System.Threading.Tasks;
using Cardbooru.Core;
using Cardbooru.Core.Entities;

namespace Cardbooru.Application.Interfaces
{
    public interface IImageCachingService
    {
        bool IsHasCache(IBooruPost booruImage, ImageSizeType imageType);

        Task CacheImageAsync(IBooruPost booruImage, 
            ImageSizeType imageType, byte[] bytes, 
            ImageSizeType sizeType, 
            CancellationToken cancellationToken = default);

        Task<byte[]> GetImageAsync(IBooruPost booruImage, 
            ImageSizeType imageType, CancellationToken cancellationToken = default);
    }
}