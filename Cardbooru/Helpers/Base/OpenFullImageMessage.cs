using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Helpers.Base
{
    class OpenFullImageMessage :
        MvxMessage
    {
        public OpenFullImageMessage(object sender, BooruImage booruImage) : base(sender) {
            BooruImage = booruImage;
        }

        public BooruImage BooruImage { get; private set; }
    }
}
