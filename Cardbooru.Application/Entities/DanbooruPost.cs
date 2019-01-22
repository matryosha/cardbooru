using System.Collections.Generic;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;

namespace Cardbooru.Application.Entities
{
    class DanbooruPost : IBooruPost
    {
        public string TagsString { get; set; }
        public string Rating { get; set; }
        public string Id { get; set; }
        public string PreviewImageUrl { get; set; }
        public string FullImageUrl { get; set; }
        public List<string> TagsList { get; set; }
        public string Hash { get; set; }
        public BooruMediaType MediaType { get; set; }
    }
}
