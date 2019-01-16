using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using Cardbooru.Helpers;
using Cardbooru.Helpers.Base;
using Cardbooru.Models.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.BrowseImages
{
    public class BrowseImagesViewModel : 
        INotifyPropertyChanged, IUserControlViewModel {


        private IDisposable _settingsToken;
        private IDisposable _resetToken;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private IMvxMessenger _messenger;
        //private int _currentQueryPage;
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

        private bool _isLoaded;
        public bool IsLoaded {
            get => _isLoaded;
            set {
                _isLoaded = value;
                OnPropertyChanged("IsLoaded");
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

        private PageNumberKeeper _pageNumberKeeper;
        public PageNumberKeeper PageNumberKeeper
        {
            get => _pageNumberKeeper ?? (_pageNumberKeeper = new PageNumberKeeper
            {
                NextQueriedPage = 1,
                DisplayedPage = 1
            });
            set
            {
                _pageNumberKeeper = value;
                OnPropertyChanged("PageNumberKeeper");
            }
        }

        public object CurrentScroll { get; set; }

        public BooruSiteType CurrentSite { get; private set; }

        public ObservableCollection<BooruImageModelBase> BooruImages { get; set; } = 
            new ObservableCollection<BooruImageModelBase>();
        

        public BrowseImagesViewModel(IMvxMessenger messenger)
        {
            _messenger = messenger;
            _settingsToken = _messenger.Subscribe<SettingsMessage>(SiteChanged);
            _resetToken = _messenger.Subscribe<ResetBooruImagesMessage>(DropImages);
        }


        #region Commands
        /// <summary>
        /// Query images from booru
        /// </summary>
        private RelayCommand _loadPreviewImages;
        public RelayCommand LoadCommand => _loadPreviewImages ?? 
            (_loadPreviewImages = new RelayCommand(async o =>
            { 
                if (_cancellationTokenSource.IsCancellationRequested) IsLoading = false;
                if (IsLoading) return;
                _cancellationTokenSource = new CancellationTokenSource();
                //if(PageNumberKeeper.NextQueriedPage==1) BooruImages.Clear();
                IsLoading = true;
                IsLoaded = true;
                try
                {
                    while (PageNumberKeeper.AddedImagesCount < Properties.Settings.Default.NumberOfPicPerRequest
                           && _cancellationTokenSource.IsCancellationRequested == false)
                    {
                        await BooruWorker.FillBooruImages(PageNumberKeeper, BooruImages, CurrentSite,
                            _cancellationTokenSource.Token);
                    }
                    IsLoading = false;

                } catch (HttpRequestException e)
                {
                    ToggleErrorOccured.Execute(new object());
                    ErrorInfo = e.Message;
                    IsLoading = false;
                }
                catch (OperationCanceledException)
                {

                }
                catch (Exception e) {
                    ToggleErrorOccured.Execute(new object());
                    ErrorInfo = e.Message;
                    IsLoading = false;
                }
                //_currentQueryPage++;
            }));

        private RelayCommand _openFullImageCommand;
        public RelayCommand OpenFullCommand =>
            _openFullImageCommand ??
            (_openFullImageCommand = new RelayCommand(async o =>
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var boouru = o as BooruImageModelBase;
                _messenger.Publish(new OpenFullImageMessage(this, o as BooruImageModelBase, BooruImages,
                    PageNumberKeeper.NextQueriedPage - 1));
                try
                {
                    await BooruWorker.LoadFullImage(boouru, _cancellationTokenSource.Token);
                }
                catch (HttpRequestException e)
                {
                    boouru.FullImage = null;
                    _messenger.Publish(new CloseFullImageMessage(new object()));
                    ToggleErrorOccured.Execute(new object());
                    ErrorInfo = e.Message;
                }
                catch (Exception e)
                {
                    ToggleErrorOccured.Execute(new object());
                    _messenger.Publish(new CloseFullImageMessage(new object()));
                    ErrorInfo = e.Message;
                }
            }));

        private RelayCommand _prevPageCommand;
        public RelayCommand PrevPageCommand => 
            _prevPageCommand ?? (
                _prevPageCommand = new RelayCommand(o =>
                {
                    if (PageNumberKeeper.DisplayedPage == 1) return;
                    _cancellationTokenSource.Cancel();
                    if (_pageNumberKeeper.QuriedPagesAccordance.ContainsKey(PageNumberKeeper.DisplayedPage - 1))
                    {
                        PageNumberKeeper.NextQueriedPage =
                            PageNumberKeeper.QuriedPagesAccordance[PageNumberKeeper.DisplayedPage - 1];
                        PageNumberKeeper.QuriedPagesAccordance.Remove(PageNumberKeeper.DisplayedPage - 1);
                    }
                    else if (PageNumberKeeper.DisplayedPage - 1 == 1)
                    {
                        PageNumberKeeper.ResetQueryInfo();
                    } else
                    {
                        PageNumberKeeper.NextQueriedPage = PageNumberKeeper.NextQueriedPage - PageNumberKeeper.QueriedPagesCount - 1;
                    }
                    PageNumberKeeper.DisplayedPage--;
                    BooruImages.Clear();
                    PageNumberKeeper.QueriedPagesCount = 0;
                    _pageNumberKeeper.AddedImagesCount = 0;
                    LoadCommand.Execute(null);
                }));

        private RelayCommand _nextPageCommand;
        public RelayCommand NextPageCommand =>
            _nextPageCommand ?? (
                _nextPageCommand = new RelayCommand( o => {
                    _cancellationTokenSource.Cancel();
                    if (_pageNumberKeeper.QueriedPagesCount > 1)
                    {
                        _pageNumberKeeper.QuriedPagesAccordance.Add(_pageNumberKeeper.DisplayedPage + 1,
                            _pageNumberKeeper.NextQueriedPage);
                    }
                    if (IsLoading) _pageNumberKeeper.NextQueriedPage++;
                    _pageNumberKeeper.DisplayedPage++;
                    BooruImages.Clear();
                    _pageNumberKeeper.QueriedPagesCount = 0;
                    _pageNumberKeeper.AddedImagesCount = 0;
                    LoadCommand.Execute(null);
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
            _cancellationTokenSource?.Cancel();
            BooruImages = new ObservableCollection<BooruImageModelBase>();
            CurrentSite = message.CurrentSiteSettings;
            IsLoaded = false;
            PageNumberKeeper.ResetAll();
        }

        private void DropImages(ResetBooruImagesMessage message)
        {
            _cancellationTokenSource?.Cancel();
            BooruImages = new ObservableCollection<BooruImageModelBase>();
            IsLoaded = false;
            PageNumberKeeper.ResetAll();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
 
    }
}
