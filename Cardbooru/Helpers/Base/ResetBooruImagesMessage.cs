using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Helpers.Base
{
    class ResetBooruImagesMessage : MvxMessage
    {
        public ResetBooruImagesMessage(object sender) : base(sender)
        {
        }
    }
}
