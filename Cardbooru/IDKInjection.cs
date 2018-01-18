using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru
{
    public static class IdkInjection {

        private static MvxMessengerHub _messengerHub;
        public static MvxMessengerHub MessengerHub => _messengerHub ?? (_messengerHub = new MvxMessengerHub());
    }
}
