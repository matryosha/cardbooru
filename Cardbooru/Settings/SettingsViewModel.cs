using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cardbooru.Helpers;
using Cardbooru.Helpers.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Settings
{
    public class SettingsViewModel :
        INotifyPropertyChanged, IUserControlViewModel
    {


        private bool _safeCheck;
        private bool _questionableCheck;
        private bool _explicitCheck;
        private bool _undefinedCheck;

        public bool SafeCheck
        {
            get => _safeCheck;
            set
            {
                _safeCheck = value;
                Properties.Settings.Default.SafeCheck = value;
                OnPropertyChanged("SafeCheck");
            }
        }
        public bool QuestionableCheck
        {
            get => _questionableCheck;
            set
            {
                _questionableCheck = value;
                Properties.Settings.Default.QuestionableCheck = value;
                OnPropertyChanged("QuestionableCheck");
            }
        }
        public bool ExplicitCheck
        {
            get => _explicitCheck;
            set
            {
                _explicitCheck = value;
                Properties.Settings.Default.ExplicitCheck = value;
                OnPropertyChanged("ExplicitCheck");
            }
        }
        public bool UndefinedCheck
        {
            get => _undefinedCheck;
            set
            {
                _undefinedCheck = value;
                Properties.Settings.Default.UndefinedCheck = value;
                OnPropertyChanged("UndefinedCheck");
            }
        }

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

        public SettingsViewModel()
        {
            SafeCheck = Properties.Settings.Default.SafeCheck;
            QuestionableCheck = Properties.Settings.Default.QuestionableCheck;
            ExplicitCheck = Properties.Settings.Default.ExplicitCheck;
            UndefinedCheck = Properties.Settings.Default.UndefinedCheck;
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
