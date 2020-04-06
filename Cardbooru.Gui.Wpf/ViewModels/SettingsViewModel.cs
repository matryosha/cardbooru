using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cardbooru.Application.Infrastructure.Messages;
using Cardbooru.Application.Interfaces;
using Cardbooru.Core;
using Cardbooru.Gui.Wpf.Infrastructure;
using Cardbooru.Gui.Wpf.Interfaces;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Gui.Wpf.ViewModels
{
    public class SettingsViewModel :
        IUserControlViewModel
    {
        private readonly IMvxMessenger _messenger;
        private readonly IBooruConfiguration _configuration;

        public string CacheSize { get; set; } = "Update";
        public string CachePath { get; set; } = String.Empty;
        public bool SafeCheck { get; set; }
        public bool QuestionableCheck { get; set; }
        public bool ExplicitCheck { get; set; }
        public bool UndefinedCheck { get; set; }
        public BooruSiteType CurrentSite { get; set; }

        public SettingsViewModel(IMvxMessenger messenger,
            IBooruConfiguration configuration)
        {
            _messenger = messenger;
            _configuration = configuration;
        }

        public void UpdateValues()
        {
            var ratingConfiguration = _configuration.FetchConfiguration.RatingConfiguration;
            SafeCheck = ratingConfiguration.Safe;
            QuestionableCheck = ratingConfiguration.Questionable;
            ExplicitCheck = ratingConfiguration.Explicit;

            CachePath = _configuration.CachePath;
            CurrentSite = _configuration.ActiveSite;
        }

        public async void UpdateSizeOfCache()
        {
            var size = await Task.Run(() => GetDirectorySize(CachePath)) / 1024 / 1024;
            CacheSize = $"~ {size} MB";
        }

        public void ChangeCacheDir(string path)
        {
            throw new NotImplementedException();
            CachePath = path + "\\";
            UpdateSizeOfCache();
        }

        private RelayCommand _clearDir;
        public RelayCommand ClearCacheDirectory =>
            _clearDir ?? (_clearDir = new RelayCommand(o =>
            {
                var files = Directory.GetFiles(CachePath, "*_preview");
                foreach (var file in files) File.Delete(file);
                files = Directory.GetFiles(CachePath, "*_full");
                foreach (var file in files) File.Delete(file);
                UpdateSizeOfCache();
            }));

        private RelayCommand _saveSettingsCommand;

        public RelayCommand SaveSettingsCommand =>
            _saveSettingsCommand ?? (_saveSettingsCommand = new RelayCommand(o =>
                {
                    _configuration.ActiveSite = CurrentSite;
                    _configuration.CachePath = CachePath;
                    var ratingConfiguration = _configuration.FetchConfiguration.RatingConfiguration;
                    ratingConfiguration.Explicit = ExplicitCheck;
                    ratingConfiguration.Questionable = QuestionableCheck;
                    ratingConfiguration.Safe = SafeCheck;
                    _messenger.Publish(new SettingsUpdatedMessage(this));
                }));

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(_messenger == null) return;
            if (propertyName == "UndefinedCheck" || propertyName == "SafeCheck" ||
                propertyName == "QuestionableCheck" || propertyName == "ExplicitCheck")
            {

                _messenger.Publish(new ResetBooruImagesMessage(this));
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
