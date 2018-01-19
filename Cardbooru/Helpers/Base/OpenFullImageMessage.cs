using Cardbooru;
using Cardbooru.Models.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Helpers.Base
{
    class OpenFullImageMessage :
        MvxMessage
    {
        public OpenFullImageMessage(object sender, BooruImageModelBase booruImageModel) : base(sender) {
            BooruImageModel = booruImageModel;
        }

        public BooruImageModelBase BooruImageModel { get; private set; }
    }
}
