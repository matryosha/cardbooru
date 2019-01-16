using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Exceptions;
using Cardbooru.Helpers;

namespace Cardbooru.Application.Helpers
{
    public class PostFetcherServiceHelper
    {
        private readonly UrlConfiguration _urlConfiguration;
        public PostFetcherServiceHelper(RootConfiguration configuration)
        {
            _urlConfiguration = configuration.UrlConfiguration;
        }

        public string GetPostsUrl(BooruSiteType type, int limit, int pageNumber, ICollection<string> tags)
        {
            switch (type)
            {
                case BooruSiteType.Danbooru:
                    var danbooru = _urlConfiguration.DanbooruUrlConfiguration;
                    return danbooru.BaseUrl + AssemblePostsUrl(danbooru.PostsUrl, limit, pageNumber, tags);
                case BooruSiteType.SafeBooru:
                    var safebooru = _urlConfiguration.SafebooruUrlConfiguration;
                    return safebooru.BaseUrl + AssemblePostsUrl(safebooru.PostsUrl, limit, pageNumber, tags);
                default:
                    throw new AssemblePostUrlException();
            }
        }

        private string AssemblePostsUrl(string url, int limit, int pageNumber, ICollection<string> tags)
        {
            var globbing = _urlConfiguration.GlobbingConfiguration;

            var builder = new StringBuilder(url);
            builder.Replace(globbing.Limit, limit.ToString());
            builder.Replace(globbing.PageNumber, pageNumber.ToString());
            if (tags.Any())
            {
                builder.Replace(globbing.Tags, "");
                builder.Append("&tags");
                foreach (var tag in tags) builder.Append($"+{tag}");
            }

            return builder.ToString();
        }
    }
}
