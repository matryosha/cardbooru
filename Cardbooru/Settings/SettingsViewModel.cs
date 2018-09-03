using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
        private IMvxMessenger _messenger;


        private string _cacheSize = "Update";
        public string CacheSize
        {
            get => _cacheSize;
            set
            {
                _cacheSize = value;
                OnPropertyChanged("CacheSize");
            }
        }
        private string _cachePath = String.Empty;
        public string CachePath
        {
            get => _cachePath;
            set
            {
                _cachePath = value;
                OnPropertyChanged("CachePath");
            }
        }

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
                _messenger.Publish(new SettingsMessage(this, value));
                OnPropertyChanged("CurrentSite");
                
            }
        }

        public SettingsViewModel(IMvxMessenger messenger)
        {
            SafeCheck = Properties.Settings.Default.SafeCheck;
            QuestionableCheck = Properties.Settings.Default.QuestionableCheck;
            ExplicitCheck = Properties.Settings.Default.ExplicitCheck;
            UndefinedCheck = Properties.Settings.Default.UndefinedCheck;
            _messenger = messenger;
            CachePath = Properties.Settings.Default.PathToCacheFolder;
            if(String.IsNullOrEmpty(Properties.Settings.Default.CurrentSite)) return;
            CurrentSite = (BooruType)Enum.Parse(typeof(BooruType), Properties.Settings.Default.CurrentSite);
        }

        public async void UpdateSizeOfCache()
        {
            var size = await Task.Run(() => GetDirectorySize(CachePath)) / 1024 / 1024;
            CacheSize = $"~ {size} MB";
        }

        public void ChangeCacheDir(string path)
        {
            CachePath = path + "\\";
            Properties.Settings.Default.PathToCacheFolder = CachePath;
            UpdateSizeOfCache();
        }

        private RelayCommand _clearDir;

        public RelayCommand ClearCacheDirectory => _clearDir ?? (_clearDir = new RelayCommand(o =>
        {
            var files = Directory.GetFiles(CachePath, "*_preview");
            foreach (var file in files)
            {
                File.Delete(file);
            }
            files = Directory.GetFiles(CachePath, "*_full");
            foreach (var file in files)
            {
                File.Delete(file);
            }
            UpdateSizeOfCache();
        }));

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(_messenger == null) return;
            if (propertyName == "UndefinedCheck" || propertyName == "SafeCheck" ||
                propertyName == "QuestionableCheck" || propertyName == "ExplicitCheck")
            {
                _messenger.Publish(new ResetBooruImagesMessage(this));
                GetConverter.UpdateRatingTags();
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        static long GetDirectorySize(string p)
        {
            // 1.
            // Get array of all file names.
            string[] a = Directory.GetFiles(p, "*");

            // 2.
            // Calculate total bytes of all files in a loop.
            long b = 0;
            foreach (string name in a)
            {
                // 3.
                // Use FileInfo to get length of each file.
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            // 4.
            // Return total size
            return b;
        }


    }
}
