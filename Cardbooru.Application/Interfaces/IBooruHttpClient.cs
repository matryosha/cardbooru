using System.Threading;
using System.Threading.Tasks;

namespace Cardbooru.Application.Interfaces
{
    public interface IBooruHttpClient
    {
        Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default);

        Task<byte[]> GetByteArrayAsync(string url, CancellationToken cancellationToken = default);
    }
}