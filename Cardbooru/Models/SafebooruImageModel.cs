using System;
using System.Collections.Generic;
using Cardbooru.Models.Base;
using Newtonsoft.Json;

namespace Cardbooru.Models
{
    class SafebooruImageModel : BooruImageModelBase
    {

        public new static string SiteUrl = Properties.Settings.Default.SafeBooruUrl;
        public new static string PostsUrl = Properties.Settings.Default.SafeBooruPostsUrl;
        public override string GetPostsUrl() => PostsUrl;
        public override string GetSiteUrl() => SiteUrl;

        [JsonProperty("directory")]
        private string _directoryNum;
        [JsonProperty("image")]
        private string _imageName;
        [JsonProperty("tags")]
        public override string TagsString { get=>base.TagsString; set=> base.TagsString = value; }

        [JsonProperty("rating")]
        public override string Rating
        {
            get => base.Rating;
            set
            {
                if (value == "safe")
                    base.Rating = "s";
                else base.Rating = value;
            }
        }

        [JsonProperty("id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        [JsonProperty("hash")]
        public override string Hash { get => base.Hash; set => base.Hash = value; }

        public override string PreviewImageUrl { get => base.PreviewImageUrl ?? (base.PreviewImageUrl = $"{SiteUrl}/thumbnails/{_directoryNum}/thumbnail_{_imageName}");
            set { base.PreviewImageUrl = $"{SiteUrl}/thumbnails/{_directoryNum}/thumbnail_{value}"; }
        }

        public override string FullImageUrl {
            get => base.FullImageUrl ?? (FullImageUrl = $"{SiteUrl}/images/{_directoryNum}/{_imageName}");
            set { base.FullImageUrl = $"{SiteUrl}/images/{_directoryNum}/{value}"; }
        }

    }
}
