using System.Threading;
using System.Threading.Tasks;
using Cardbooru.Core;
using Cardbooru.Core.Entities;

namespace Cardbooru.Application.Interfaces
{
    public interface IImageFetcherService
    {
        Task<byte[]> FetchImageAsync(
            IBooruPost booruPost,
            ImageSizeType imageSizeType,
            bool caching = true,
            CancellationToken cancellationToken = default);
    }
}