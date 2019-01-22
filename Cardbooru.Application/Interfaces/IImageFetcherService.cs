using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Entities;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;

namespace Cardbooru.Application.Interfaces
{
    public interface IImageFetcherService
    {
        Task<BitmapImage> FetchImageAsync(
            IBooruPost booruImage,
            ImageSizeType imageSizeType,
            bool caching = true,
            CancellationToken cancellationToken = default);
    }
}