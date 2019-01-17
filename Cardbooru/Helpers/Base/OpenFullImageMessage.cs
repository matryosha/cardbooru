using System.Collections.Generic;
using Cardbooru.Application.Entities;
using Cardbooru.Models.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Helpers.Base
{
    class OpenFullImageMessage :
        MvxMessage
    {
        public ICollection<BooruImageWrapper> BooruImageWrapperList { get; }
        public int QueryPage { get; }
        public readonly BooruImageWrapper _booruImageWrapper;
        public readonly List<BooruImageModelBase> _booruPosts;

        public OpenFullImageMessage(object sender, 
            BooruImageWrapper booruImageWrapper,
            List<BooruImageModelBase> booruPosts,
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
