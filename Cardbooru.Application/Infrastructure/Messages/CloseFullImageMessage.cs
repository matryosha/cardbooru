using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Application.Infrastructure.Messages
{
    public class CloseFullImageMessage :
        MvxMessage
    {
        public CloseFullImageMessage(object sender) : base(sender) { }
    }
}
