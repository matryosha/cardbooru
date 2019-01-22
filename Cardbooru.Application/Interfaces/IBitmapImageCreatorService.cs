using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Cardbooru.Application.Interfaces
{
    public interface IBitmapImageCreatorService
    {
        Task<BitmapImage> CreateImageAsync(byte[] bytes);
    }
}