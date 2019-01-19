using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Helpers.Base
{
    public class SettingsUpdatedMessage :
        MvxMessage {
        public SettingsUpdatedMessage(object sender) : base(sender)
        {
        }
    }
}
