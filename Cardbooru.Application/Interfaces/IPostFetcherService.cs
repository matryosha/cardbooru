using System.Collections.Generic;
using System.Threading.Tasks;
using Cardbooru.Application.Entities;
using Cardbooru.Domain;

namespace Cardbooru.Application.Interfaces
{
    public interface IPostFetcherService
    {
        /// <summary>
        /// Return JSON string from fetched url
        /// </summary>
        Task<string> FetchPostsAsync(BooruSiteType type, int limit = 100, int pageNumber = 1, ICollection<string> tags = default);
    }
}