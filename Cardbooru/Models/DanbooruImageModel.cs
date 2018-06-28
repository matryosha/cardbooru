using Cardbooru.Models.Base;
using Newtonsoft.Json;

namespace Cardbooru.Models
{
    public class DanbooruImageModel : BooruImageModelBase
    {

        public static string SiteUrl = Properties.Settings.Default.DanbooruUrl;
        public static string PostsUrl = Properties.Settings.Default.DanbooruPostsUrl;
        public override string GetPostsUrl() => PostsUrl;
        public override string GetSiteUrl() => SiteUrl;

        [JsonProperty("id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        [JsonProperty("preview_file_url")]
        public override string PreviewImageUrl { get => base.PreviewImageUrl; set => base.PreviewImageUrl = value; }
        [JsonProperty("large_file_url")]
        public override string FullImageUrl { get => base.FullImageUrl; set => base.FullImageUrl = value; }
        [JsonProperty("md5")]
        public override string Hash { get => base.Hash; set => base.Hash = value; }
        [JsonProperty("tag_string")]
        public override string TagsString { get; set; }
        [JsonProperty("rating")]
        public override string Rating { get => base.Rating; set => base.Rating = value; }
    }
}
