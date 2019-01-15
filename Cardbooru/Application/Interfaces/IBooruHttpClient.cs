using System.Threading.Tasks;

namespace Cardbooru.Application.Interfaces
{
    public interface IBooruHttpClient
    {
        Task<string> GetStringAsync(string url);

        Task<byte[]> GetByteArrayAsync(string url);
    }
}