namespace Cardbooru.Application.Configurations
{
    public class FetchConfiguration
    {
        public int PostLimit { get; set; }
        public RatingConfiguration RatingConfiguration { get; set; }
        public GlobbingConfiguration GlobbingConfiguration { get; set; }
        public BooruSiteUrlConfiguration DanbooruUrlConfiguration { get; set; }
        public BooruSiteUrlConfiguration SafebooruUrlConfiguration { get; set; }
    }

    public class RatingConfiguration
    {
        public bool Safe { get; set; }
        public bool Explicit { get; set; }
        public bool Questionable { get; set; }
    }
}