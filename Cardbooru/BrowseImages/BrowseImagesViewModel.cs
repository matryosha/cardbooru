using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Cardbooru.Helpers;
using Cardbooru.Helpers.Base;
using Cardbooru.Models.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.BrowseImages
{
    public class BrowseImagesViewModel : 
        INotifyPropertyChanged, IUserControlViewModel {


        private IDisposable _settingsToken;
        private int _currentPage;
        private bool _isLoading;
        /// <summary>
        /// Indicate whether pictures loading to list or not
        /// </summary>
        public bool IsLoading { get => _isLoading;
            set {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }

        private bool _isErrorOccured;
        public bool IsErrorOccured {
            get => _isErrorOccured;
            set {
                _isErrorOccured = value;
                OnPropertyChanged("IsErrorOccured");
            }
        }

        private string _errorInfo;
        public string ErrorInfo {
            get => _errorInfo;
            set {
                _errorInfo = value;
                OnPropertyChanged("ErrorInfo");
            }
        }

        private OpenFullImageMessage _openFullImageMessage;

        public object CurrentScroll { get; set; }

        private BooruWorker booruWorker; // TODO Make Interface and make static?

        public BooruType CurrentSite { get; private set; }

        public IMvxMessenger Messenger { get; }
        public ObservableCollection<BooruImageModelBase> BooruImages { get; set; } = 
            new ObservableCollection<BooruImageModelBase>();
        

        public BrowseImagesViewModel() {
            Messenger = IdkInjection.MessengerHub;
            _currentPage = 1;
            booruWorker = new BooruWorker();
            _settingsToken = Messenger.Subscribe<SettingsMessage>(SiteChanged);
        }


        #region Commands

        private RelayCommand _loadPreviewImages;
        public RelayCommand LoadCommand => _loadPreviewImages ?? 
            (_loadPreviewImages = new RelayCommand(async o => {
                if(IsLoading) return;
                if(_currentPage==1) BooruImages.Clear();
                IsLoading = true;
                try {
                    await booruWorker.FillBooruImages(_currentPage, BooruImages, CurrentSite);
                }
                catch (HttpRequestException e) {
                    ToggleErrorOccured.Execute(new object());
                    ErrorInfo = e.Message;
                }
                catch (Exception e) {
                    ToggleErrorOccured.Execute(new object());
                    ErrorInfo = e.Message;
                }
                finally {
                    IsLoading = false;
                }
                _currentPage++;
            }));

        private RelayCommand _openFullImageCommand;

        public RelayCommand OpenFullCommand => _openFullImageCommand ??
                                               (_openFullImageCommand = new RelayCommand(async o => {
                                                   var boouru = o as BooruImageModelBase;
                                                   _openFullImageMessage = new OpenFullImageMessage(this, o as BooruImageModelBase);
                                                   Messenger.Publish(_openFullImageMessage);
                                                   try {
                                                       await booruWorker.LoadFullImage(boouru);
                                                   }
                                                   catch (HttpRequestException e) {
                                                       boouru.FullImage = null;
                                                       Messenger.Publish(new CloseFullImageMessage(new object()));
                                                       ToggleErrorOccured.Execute(new object());
                                                       ErrorInfo = e.Message;
                                                   }
                                                   catch (Exception e) {
                                                       ToggleErrorOccured.Execute(new object());
                                                       ErrorInfo = e.Message;
                                                   }

                                               }));

        private RelayCommand _toggleErrorOccured;

        public RelayCommand ToggleErrorOccured => _toggleErrorOccured ?? (_toggleErrorOccured = new RelayCommand(o => {
            IsErrorOccured = !IsErrorOccured;
            ErrorInfo = string.Empty;
        }));

        private RelayCommand _loadStateCommand;
        public RelayCommand LoadStateCommand => _loadStateCommand ?? (_loadStateCommand = new RelayCommand(o => { }));
        #endregion

        private void SiteChanged(SettingsMessage message) {
            if(message.CurrentSiteSettings == CurrentSite) return;
            BooruImages = new ObservableCollection<BooruImageModelBase>();
            CurrentSite = message.CurrentSiteSettings;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        
    }
}
