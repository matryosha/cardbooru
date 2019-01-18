namespace Cardbooru.Application.Configurations
{
    public class FetchConfiguration
    {
        public int PostLimit { get; set; }
        public GlobbingConfiguration GlobbingConfiguration { get; set; }
        public BooruSiteUrlConfiguration DanbooruUrlConfiguration { get; set; }
        public BooruSiteUrlConfiguration SafebooruUrlConfiguration { get; set; }
    }
}