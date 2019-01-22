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
        public readonly BooruImageWrapper _booruImageWrapper;
        public readonly List<IBooruPost> _booruPosts;

        public OpenFullImageMessage(object sender, 
            BooruImageWrapper booruImageWrapper,
            List<IBooruPost> booruPosts,
            ICollection<BooruImageWrapper> booruImageWrapperList = default,
            int queryPage = -1) 
            : base(sender)
        {
            BooruImageWrapperList = booruImageWrapperList;
            QueryPage = queryPage;
            _booruImageWrapper = booruImageWrapper;
            _booruPosts = booruPosts;
        }

    }
}
