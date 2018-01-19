/// 
/// 
/// 
/// https://www.mvvmcross.com/documentation/plugins/messenger
/// There is the tip how to use Messenger properly using LazyConstruct
/// but problem is I totaly not understand how exactly 
/// I should use that method.
/// 
///  "Add the following to app.cs in the Initialize() method" -- fckn where?
/// 
/// 
/// Thus, I created this class for doing the same thing but in a wrong way
/// 
///  
/// 
using MvvmCross.Plugins.Messenger;

namespace Cardbooru
{
    public static class IdkInjection {

        private static MvxMessengerHub _messengerHub;
        public static MvxMessengerHub MessengerHub => _messengerHub ?? (_messengerHub = new MvxMessengerHub());
    }
}
