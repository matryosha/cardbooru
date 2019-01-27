using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cardbooru.Application.Entities;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;

namespace Cardbooru.Application.Interfaces
{
    //ToDo refactor
    public interface IPostCollectionManager
    {
        Task<List<BooruImage>> GetImagesAsync(
            BooruSiteType booruSiteType,
            ImageSizeType imageType,
            ICollection<IBooruPost> collection,
            CancellationToken cancellationToken = default);
    }
}