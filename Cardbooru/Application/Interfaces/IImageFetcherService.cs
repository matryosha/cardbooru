using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Helpers;
using Cardbooru.Models.Base;

namespace Cardbooru.Application.Interfaces
{
    public interface IImageFetcherService
    {
        Task<BitmapImage> FetchImageAsync(
            BooruImageModelBase booruImage,
            ImageSizeType imageSizeType,
            bool caching = true,
            CancellationToken token = default);
    }
}