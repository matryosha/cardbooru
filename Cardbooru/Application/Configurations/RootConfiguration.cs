using Cardbooru.Helpers;

namespace Cardbooru.Application.Configurations
{
    public class RootConfiguration
    {
        public RootConfiguration()
        {
            
        }
        public UrlConfiguration UrlConfiguration { get; set; }
        public string CachePath { get; set; }
        public BooruSiteType ActiveSite { get; set; }
        public bool ImageCaching { get; set; }
    }
}