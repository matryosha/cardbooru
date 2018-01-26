using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cardbooru.Helpers;
using Cardbooru.Helpers.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Settings
{
    public class SettingsViewModel :
        INotifyPropertyChanged, IUserControlViewModel {


        private BooruType _currentSite;
        public BooruType CurrentSite {
            get => _currentSite;
            set {
                _currentSite = value;
                Properties.Settings.Default.CurrentSite = value.ToString();
                Messenger.Publish(new SettingsMessage(this, value));
                OnPropertyChanged("CurrentSite");
            }
        }

        public SettingsViewModel() {
            Messenger = IdkInjection.MessengerHub;
            if(String.IsNullOrEmpty(Properties.Settings.Default.CurrentSite)) return;
            CurrentSite = (BooruType)Enum.Parse(typeof(BooruType), Properties.Settings.Default.CurrentSite);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public IMvxMessenger Messenger { get; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }





    }
}
