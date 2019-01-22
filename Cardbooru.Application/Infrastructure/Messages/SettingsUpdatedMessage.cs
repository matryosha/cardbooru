using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Application.Infrastructure.Messages
{
    public class SettingsUpdatedMessage :
        MvxMessage {
        public SettingsUpdatedMessage(object sender) : base(sender)
        {
        }
    }
}
