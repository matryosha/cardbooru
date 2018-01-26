using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Helpers.Base
{
    public class SettingsMessage :
        MvxMessage {

        public BooruType CurrentSiteSettings { get; }

        public SettingsMessage(object sender, BooruType type) : base(sender) {
            CurrentSiteSettings = type;
        }
    }
}
