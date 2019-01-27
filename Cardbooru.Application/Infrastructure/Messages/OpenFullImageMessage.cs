using Cardbooru.Application.Entities;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Application.Infrastructure.Messages
{
    public class OpenFullImageMessage :
        MvxMessage
    {
        public BooruPostsProvider Provider { get; }
        public readonly BooruImageWrapper BooruImage;

        public OpenFullImageMessage(object sender, 
            BooruImageWrapper booruImage,
            BooruPostsProvider provider) 
            : base(sender)
        {
            Provider = provider;
            BooruImage = booruImage;
        }

    }
}
