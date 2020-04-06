
using System.Collections.Generic;
using Cardbooru.Core;
using Cardbooru.Core.Entities;

namespace Cardbooru.Application.Entities
{
    public class SafebooruPost : IBooruPost
    {
        public string TagsString { get; set; }
        public string Rating { get; set; }
        public string Id { get; set; }
        public string PreviewImageUrl { get; set; }
        public string FullImageUrl { get; set; }
        public List<string> TagsList { get; set; }
        public string Hash { get; set; }
        public BooruMediaType MediaType { get; set; }
        public string ImageName { get; set; }
        public string Directory { get; set; }

    }
}
