using System;
using System.Reflection;
using Cardbooru.Application.Interfaces;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cardbooru.Application.Infrastructure
{
    public class BooruContractResolver : DefaultContractResolver
    {
        private readonly IBooruConfiguration _configuration;
        private BooruSiteType _siteType;
        private readonly BooruPostMapping _booruPostMapping;

        public BooruContractResolver(IBooruConfiguration configuration)
        {
            _configuration = configuration;
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

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.DeclaringType == typeof(IBooruPost))
            {
                if (property.PropertyName.Equals("LongPropertyName", StringComparison.OrdinalIgnoreCase))
                {
                    property.PropertyName = "Short";
                }
            }
            return property;
        }
    }
}
