namespace Cardbooru.Application.Configurations
{
    public class RootConfiguration
    {
        public RootConfiguration()
        {
            
        }
        public UrlConfiguration UrlConfiguration { get; set; }
        public string CachePath { get; set; }
    }
}