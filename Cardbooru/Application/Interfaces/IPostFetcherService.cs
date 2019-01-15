using System.Collections.Generic;
using System.Threading.Tasks;
using Cardbooru.Helpers;

namespace Cardbooru.Application.Interfaces
{
    public interface IPostFetcherService
    {
        /// <summary>
        /// Return JSON string from fetched url
        /// </summary>
        Task<string> FetchPostsAsync(BooruType type, int limit, int pageNumber, ICollection<string> tags);
    }
}