using System;
using System.Collections.Generic;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Infrastructure;
using Cardbooru.Application.Interfaces;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;

namespace Cardbooru.Application.Managers
{
    class BooruPostManager : IBooruPostManager
    {
        private readonly CustomJsonSerializer _jsonSerializer;
        private readonly IBooruConfiguration _configuration;
        private readonly BooruContractResolver _booruContractResolver;

        public BooruPostManager(CustomJsonSerializer jsonSerializer, 
            IBooruConfiguration configuration)
        {
            _jsonSerializer = jsonSerializer;
            _configuration = configuration;
            _booruContractResolver = 
                new BooruContractResolver(_configuration);
        }

        public List<IBooruPost> DeserializePosts(BooruSiteType type, string posts)
        {
            switch (type)
            {
                case BooruSiteType.Danbooru:
                    _booruContractResolver.SetBooruSite(BooruSiteType.Danbooru);
                    return new List<IBooruPost>(
                        _jsonSerializer.DeserializeBooruPosts<List<DanbooruPost>, DanbooruPost>(posts, _booruContractResolver));
                case BooruSiteType.SafeBooru:
                    _booruContractResolver.SetBooruSite(BooruSiteType.SafeBooru);
                    return new List<IBooruPost>(
                        _jsonSerializer.DeserializeBooruPosts<List<SafebooruPost>, SafebooruPost>(posts, _booruContractResolver));
                case BooruSiteType.Gelbooru:
                    _booruContractResolver.SetBooruSite(BooruSiteType.Gelbooru);
                    return new List<IBooruPost>(
                        _jsonSerializer.DeserializeBooruPosts<List<GelbooruPost>, GelbooruPost>(posts, _booruContractResolver));
            }

            throw new Exception("Unknown booru type for deserialize");
        }

        public string GetRatingTagString(BooruSiteType type, RatingConfiguration ratingConfiguration)
        {
            //ToDo maybe configure rating tag value in configuration file
            switch (ratingConfiguration.GetEnabledRatingTagCount())
            {
                case 1:
                {
                    if (ratingConfiguration.Safe)
                        return "rating%3Asafe";
                    if (ratingConfiguration.Explicit)
                    {
                        if (type == BooruSiteType.SafeBooru)
                            return String.Empty;
                        if (type == BooruSiteType.Gelbooru)
                            return "rating%3Aexplicit";
                        return "rating%3Ae";
                    }
                    return "rating%3Aquestionable";
                }
                case 2:
                {
                    if (!ratingConfiguration.Safe)
                        return "-rating%3Asafe";
                    if (!ratingConfiguration.Explicit)
                        return "-rating%3Ae";
                    return "-rating%3Aquestionable";
                }
                default: return String.Empty;
            }
        }
    }
}
