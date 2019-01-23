using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Interfaces;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cardbooru.Application.Infrastructure
{
    class CustomJsonSerializer : JsonSerializer
    {
        private readonly IBooruConfiguration _configuration;

        internal class BooruPostFinalizer
        {
            private readonly IBooruConfiguration _configuration;
            private readonly Dictionary<Type, Action<IBooruPost>> _switchTypes;

            public BooruPostFinalizer(IBooruConfiguration configuration)
            {
                _configuration = configuration;
                _switchTypes = new Dictionary<Type, Action<IBooruPost>>
                {
                    { typeof(DanbooruPost), post => FinalizeDanbooru(post as DanbooruPost) },
                    { typeof(SafebooruPost), post =>  FinalizeSafebooru(post as SafebooruPost)},
                    { typeof(GelbooruPost), post =>  FinalizeGelbooru(post as GelbooruPost) }
                };
            }

            /// <summary>
            /// Setting on booru post necessary properties
            /// </summary>
            public void FinalizeBooruPost<T>(T post) where T : IBooruPost
            {
                _switchTypes[typeof(T)](post);
            }

            private void FinalizeDanbooru(DanbooruPost post)
            {
                try
                {
                    post.MediaType = GetBooruMediaType(post.FullImageUrl);
                }
                catch (Exception e)
                {

                }
            }

            private void FinalizeSafebooru(SafebooruPost post)
            {
                var siteUrl = _configuration.FetchConfiguration.SafebooruUrlConfiguration.BaseUrl;
                post.PreviewImageUrl = $"{siteUrl}/thumbnails/{post.Directory}/thumbnail_{post.ImageName}";
                post.FullImageUrl = $"{siteUrl}/images/{post.Directory}/{post.ImageName}";
                post.MediaType = GetBooruMediaType(post.ImageName);
            }

            private void FinalizeGelbooru(GelbooruPost post)
            {
                //Getting a subdomain
                //var regex = Regex.Match(post.FullImageUrl, @":\/\/(\w+)\.\w+\.");
                //var subServer = regex.Groups[1].Value;
                var siteUrl = _configuration.FetchConfiguration.GelbooruUrlConfiguration.BaseUrl;

                post.PreviewImageUrl = $"{siteUrl}/thumbnails/{post.Directory}/thumbnail_{post.ImageName}";
                post.MediaType = GetBooruMediaType(post.ImageName);
            }

            private static BooruMediaType GetBooruMediaType(string imageName)
            {
                if (imageName == null) return BooruMediaType.Unknown;
                var imageNameParts = imageName.Split('.');
                var mediaExtension = imageNameParts[imageNameParts.Length - 1];
                switch (mediaExtension)
                {
                    case "jpg":
                    case "jpeg":
                        return BooruMediaType.Jpeg;
                    case "png":
                        return BooruMediaType.Png;
                    case "gif":
                        return BooruMediaType.Gif;
                    default:
                        return BooruMediaType.Unknown;
                }
            }
        }

        private BooruPostFinalizer _booruPostFinalizer;
        public CustomJsonSerializer(IBooruConfiguration configuration)
        {
            _configuration = configuration;
            _booruPostFinalizer = new BooruPostFinalizer(configuration);
        }

        public TList DeserializeBooruPosts<TList, TBooruPost>(string value, 
            IContractResolver contractResolver) 
            where TList: IList 
            where TBooruPost: class, IBooruPost  
        {
            base.ContractResolver = contractResolver;
            TList result = default(TList);
            using (JsonTextReader reader = new JsonTextReader(new StringReader(value)))
            {
                result = base.Deserialize<TList>(reader);
            }

            foreach (var obj in result)
            {
                var post = obj as TBooruPost;
                _booruPostFinalizer.FinalizeBooruPost(post);
            }

            return result;
        }
    }

    
}
