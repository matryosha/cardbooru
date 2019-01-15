namespace Cardbooru.Application.Configurations
{
    public class UrlConfiguration
    {
        public int PostLimit { get; set; }
        public GlobbingConfiguration GlobbingConfiguration { get; set; }
        public DanbooruUrlConfiguration DanbooruUrlConfiguration { get; set; }
        public SafebooruUrlConfiguration SafebooruUrlConfiguration { get; set; }
    }
}