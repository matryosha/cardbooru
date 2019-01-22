using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Exceptions;
using Cardbooru.Application.Interfaces;
using Cardbooru.Domain;

namespace Cardbooru.Application.Helpers
{
    public class PostFetcherServiceHelper
    {
        private readonly FetchConfiguration _fetchConfiguration;
        public PostFetcherServiceHelper(IBooruConfiguration configuration)
        {
            _fetchConfiguration = configuration.FetchConfiguration;
        }

        public string GetPostsUrl(BooruSiteType type, int limit, int pageNumber, ICollection<string> tags)
        {
            switch (type)
            {
                case BooruSiteType.Danbooru:
                    var danbooru = _fetchConfiguration.DanbooruUrlConfiguration;
                    return danbooru.BaseUrl + AssemblePostsUrl(danbooru.PostsUrl, limit, pageNumber, tags);
                case BooruSiteType.SafeBooru:
                    var safebooru = _fetchConfiguration.SafebooruUrlConfiguration;
                    return safebooru.BaseUrl + AssemblePostsUrl(safebooru.PostsUrl, limit, pageNumber, tags);
                case BooruSiteType.Gelbooru:
                    var gelbooru = _fetchConfiguration.GelbooruUrlConfiguration;
                    return gelbooru.BaseUrl + AssemblePostsUrl(gelbooru.PostsUrl, limit, pageNumber, tags);

                default:
                    throw new AssemblePostUrlException();
            }
        }

        private string AssemblePostsUrl(string url, int limit, int pageNumber, ICollection<string> tags)
        {
            var globbing = _fetchConfiguration.GlobbingConfiguration;

            var builder = new StringBuilder(url);
            builder.Replace(globbing.Limit, limit.ToString());
            builder.Replace(globbing.PageNumber, pageNumber.ToString()); 
            if (tags != null && tags.Any())
            {
                var tagsBuilder = new StringBuilder("&tags=");
                foreach (var tag in tags) tagsBuilder.Append($"+{tag}");
                builder.Replace(globbing.Tags, tagsBuilder.ToString());
            }

            return builder.ToString();
        }
    }
}
