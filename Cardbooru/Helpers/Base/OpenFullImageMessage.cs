using System.Collections.ObjectModel;
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

        public OpenFullImageMessage(object sender, BooruImageModelBase booruImageModel, ObservableCollection<BooruImageModelBase> booruImageCollection) : base(sender) {
            BooruImageModel = booruImageModel;
            BooruImageCollection = booruImageCollection;
        }

        public BooruImageModelBase BooruImageModel { get; private set; }

        public ObservableCollection<BooruImageModelBase> BooruImageCollection { get; }
    }
}
