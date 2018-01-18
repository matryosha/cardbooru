using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Helpers.Base
{
    class CloseFullImageMessage :
        MvxMessage
    {
        public CloseFullImageMessage(object sender) : base(sender) { }
    }
}
