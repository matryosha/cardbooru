using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Helpers.Base
{
    public class SettingsMessage :
        MvxMessage {

        public BooruSiteType CurrentSiteSettings { get; }

        public SettingsMessage(object sender, BooruSiteType type) : base(sender) {
            CurrentSiteSettings = type;
        }
    }
}
