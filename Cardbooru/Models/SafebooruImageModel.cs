using Cardbooru.Models.Base;
using Newtonsoft.Json;

namespace Cardbooru.Models
{
    class SafebooruImageModel : BooruImageModelBase {

        [JsonProperty("directory")]
        private string _directoryNum;
        [JsonProperty("image")]
        private string _imageName; 
        [JsonProperty("id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        public override string PreviewImageUrl { get => base.PreviewImageUrl ?? (base.PreviewImageUrl = $"/thumbnails/{_directoryNum}/thumbnail_{_imageName}");
            set { base.PreviewImageUrl = $"/thumbnails/{_directoryNum}/thumbnail_{value}"; }
        }
        public override string FullImageUrl { get => base.FullImageUrl ?? (FullImageUrl = $"/images/{_directoryNum}/{_imageName}");
            set { base.FullImageUrl = $"/images/{_directoryNum}/{value}"; }
        }
        [JsonProperty("hash")]
        public override string Hash { get => base.Hash; set => base.Hash = value; }

    }
}
