using System;
using System.Collections.Generic;
using Cardbooru.Domain;

namespace Cardbooru.Application.Infrastructure
{
    class BooruPostMapping
    {
        private Dictionary<string, string> _danbooruPropertyMappings;
        private Dictionary<string, string> _safebooruPropertyMappings;
        private Dictionary<string, string> _gelbooruPropertyMappings;

        public Dictionary<string, string> DanbooruPropertyMappings =>
            _danbooruPropertyMappings ?? (
                _danbooruPropertyMappings =
                    new Dictionary<string, string>
                    {
                        {"Id", "id"},
                        {"PreviewImageUrl", "preview_file_url"},
                        {"FullImageUrl", "large_file_url"},
                        {"Hash", "md5"},
                        {"TagsString", "tag_string"},
                        {"Rating", "rating"}
                    });

        public Dictionary<string, string> SafebooruPropertyMappings =>
            _safebooruPropertyMappings ?? (_safebooruPropertyMappings =
                new Dictionary<string, string>
                {
                    {"Id", "id"},
                    {"Hash", "hash"},
                    {"ImageName", "image"},
                    {"Directory", "directory"},
                    {"TagsString", "tags"},
                    {"Rating", "rating"}
                });

        public Dictionary<string, string> GelbooruPropertyMappings =>
            _gelbooruPropertyMappings ?? (_gelbooruPropertyMappings =
                new Dictionary<string, string>
                {
                    {"Id", "id"},
                    {"Hash", "hash"},
                    {"ImageName", "image"},
                    {"Directory", "directory"},
                    {"TagsString", "tags"},
                    {"Rating", "rating"},
                    {"FullImageUrl", "file_url"}
                });

        public Dictionary<string, string> GetPropertyMapping(BooruSiteType booruSiteType)
        {
            switch (booruSiteType)
            {
                case BooruSiteType.Danbooru: return DanbooruPropertyMappings;
                case BooruSiteType.Gelbooru: return GelbooruPropertyMappings;
                case BooruSiteType.SafeBooru: return SafebooruPropertyMappings;
            }
            throw new NotImplementedException($"Unknown booru site type: {booruSiteType}");
        }
    }
}
