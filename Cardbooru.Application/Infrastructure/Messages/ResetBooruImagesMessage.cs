using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Application.Infrastructure.Messages
{
    public class ResetBooruImagesMessage : MvxMessage
    {
        public ResetBooruImagesMessage(object sender) : base(sender)
        {
        }
    }
}
