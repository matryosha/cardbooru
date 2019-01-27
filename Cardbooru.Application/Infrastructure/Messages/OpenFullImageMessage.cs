using System.Collections.Generic;
using Cardbooru.Application.Entities;
using Cardbooru.Domain.Entities;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Application.Infrastructure.Messages
{
    public class OpenFullImageMessage :
        MvxMessage
    {
        public ICollection<BooruImageWrapper> BooruImageWrapperList { get; }
        public int QueryPage { get; }
        public readonly BooruImageWrapper BooruImage;
        public readonly List<IBooruPost> _booruPosts;

        public OpenFullImageMessage(object sender, 
            BooruImageWrapper booruImage,
            List<IBooruPost> booruPosts,
            ICollection<BooruImageWrapper> booruImageWrapperList = default,
            int queryPage = -1) 
            : base(sender)
        {
            BooruImageWrapperList = booruImageWrapperList;
            QueryPage = queryPage;
            BooruImage = booruImage;
            _booruPosts = booruPosts;
        }

    }
}
