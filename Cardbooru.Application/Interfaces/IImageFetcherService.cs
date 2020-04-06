using System.Threading;
using System.Threading.Tasks;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;

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