using System.Threading.Tasks;
using Cardbooru.Application.Configurations;
using Cardbooru.Domain;

namespace Cardbooru.Application.Interfaces
{
    public interface IBooruConfiguration
    {
        FetchConfiguration FetchConfiguration { get; set; }
        string CachePath { get; set; }
        BooruSiteType ActiveSite { get; set; }
        bool ImageCaching { get; set; }  
        Task SaveConfiguration();
    }
}