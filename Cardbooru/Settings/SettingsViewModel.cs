using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardbooru.Helpers.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Settings
{
    public class SettingsViewModel :
        INotifyPropertyChanged, IUserControlViewModel {

        public event PropertyChangedEventHandler PropertyChanged;
        public IMvxMessenger Messenger { get; }







    }
}
