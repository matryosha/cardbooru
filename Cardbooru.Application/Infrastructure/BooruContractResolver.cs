using Cardbooru.Domain;
using Newtonsoft.Json.Serialization;

namespace Cardbooru.Application.Infrastructure
{
    public class BooruContractResolver : DefaultContractResolver
    {
        private BooruSiteType _siteType;
        private readonly BooruPostMapping _booruPostMapping;

        public BooruContractResolver()
        {
            _booruPostMapping = new BooruPostMapping();
        }

        public void SetBooruSite(BooruSiteType booruSiteType)
        {
            _siteType = booruSiteType;
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            var propertyMapping = _booruPostMapping.GetPropertyMapping(_siteType);
            var resolved = propertyMapping.TryGetValue(propertyName, out var resolvedName);
            return resolved ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }
}
