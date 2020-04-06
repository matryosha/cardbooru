using System.Collections.Generic;
using Cardbooru.Application.Configurations;
using Cardbooru.Core;
using Cardbooru.Core.Entities;

namespace Cardbooru.Application.Interfaces
{
    public interface IBooruPostManager
    {
        List<IBooruPost> DeserializePosts(BooruSiteType type, string posts);
        /// <summary>
        /// Assemble string which can be used in tags depends on rating configuration
        /// </summary>
        string GetRatingTagString(BooruSiteType type, RatingConfiguration ratingConfiguration);
    }
}