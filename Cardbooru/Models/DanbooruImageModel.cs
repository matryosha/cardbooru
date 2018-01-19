using System.Windows.Controls;
using Cardbooru.Models.Base;
using Newtonsoft.Json;


namespace Cardbooru.Models
{
    public class DanbooruImageModel :
        BooruImageModelBase
    {
        [JsonProperty("id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        [JsonProperty("preview_file_url")]
        public override string PreviewImageUrl { get => base.PreviewImageUrl; set => base.PreviewImageUrl = value; }
        [JsonProperty("large_file_url")]
        public override string FullImageUrl { get => base.FullImageUrl; set => base.FullImageUrl = value; }
        [JsonProperty("md5")]
        public override string Hash { get => base.Hash; set => base.Hash = value; }
        public override Image PreviewImage { get => base.PreviewImage; set => base.PreviewImage = value; }
        public override Image FullImage { get => base.FullImage; set => base.FullImage = value; }
    }
}
