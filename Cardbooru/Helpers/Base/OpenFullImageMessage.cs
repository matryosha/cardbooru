using Cardbooru;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Helpers.Base
{
    class OpenFullImageMessage :
        MvxMessage
    {
        public OpenFullImageMessage(object sender, BooruImageModel booruImageModel) : base(sender) {
            BooruImageModel = booruImageModel;
        }

        public BooruImageModel BooruImageModel { get; private set; }
    }
}
