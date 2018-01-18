using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Helpers.Base
{
    public interface IUserControlViewModel {
        IMvxMessenger Messenger { get; }
    }
}
