using System.Collections.Generic;

namespace Cardbooru.Domain.Entities
{
    public interface IBooruPost 
    {
        string TagsString { get; set; }
        string Rating { get; set; }
        string Id { get; set; }
        string PreviewImageUrl { get; set; }
        string FullImageUrl { get; set; }
        List<string> TagsList { get; set; } 
        string Hash { get; set; }
        BooruMediaType MediaType { get; set; }
    }
}
