using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cardbooru.Application.Entities;
using Cardbooru.Helpers;
using Cardbooru.Models.Base;

namespace Cardbooru.Application.Interfaces
{
    public interface IPostCollectionManager
    {
        ICollection<BooruImageModelBase> DeserializePosts(BooruSiteType type, string posts);

        Task<ICollection<BooruImageWrapper>> GetImagesAsync(
            BooruSiteType booruSiteType,
            ImageSizeType imageType,
            ICollection<BooruImageModelBase> collection,
            CancellationToken cancellationToken = default);
    }
}