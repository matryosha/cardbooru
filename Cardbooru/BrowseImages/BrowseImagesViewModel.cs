using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Interfaces;
using Cardbooru.Helpers;
using Cardbooru.Helpers.Base;
using Cardbooru.Models;
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
        private IPostCollectionManager _postCollectionManager;
        private IImageFetcherService _imageFetcherService;
        private IPostFetcherService _postFetcherService;
        private RootConfiguration _configuration;
        private List<BooruImageModelBase> _currentPageBooruPosts;

        /// <summary>
        /// Uses by new load page command
        /// </summary>
        private int _queryPage = 1;
        public int QueryPage
        {
            get => _queryPage;
            set
            {
                _queryPage = value;
                OnPropertyChanged("QueryPage");
            }
        }
        private bool _isProcessing;
        /// <summary>
        /// Indicate whether something something doing a thing
        /// </summary>
        public bool IsProcessing { get => _isProcessing;
            set {
                _isProcessing = value;
                OnPropertyChanged("IsProcessing");
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

        [Obsolete]
        private PageNumberKeeper _pageNumberKeeper;
        [Obsolete]
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
        

        public ObservableCollection<BooruImageWrapper> NewBooruImages { get; set; } =
            new ObservableCollection<BooruImageWrapper>();

        public BrowseImagesViewModel(IMvxMessenger messenger,
            IPostCollectionManager postCollectionManager,
            IImageFetcherService imageFetcherService,
            IPostFetcherService postFetcherService,
            RootConfiguration configuration)
        {
            _messenger = messenger;
            _imageFetcherService = imageFetcherService;
            _postCollectionManager = postCollectionManager;
            _postFetcherService = postFetcherService;
            _configuration = configuration;
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
                //if (_cancellationTokenSource.IsCancellationRequested) IsProcessing = false;
                //if (IsProcessing) return;
                NewBooruImages.Clear();
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;
                IsProcessing = true;
                try
                {
                    var postsString = await _postFetcherService.FetchPostsAsync(_configuration.ActiveSite,
                        _configuration.UrlConfiguration.PostLimit, QueryPage);

                    _currentPageBooruPosts =  _postCollectionManager.DeserializePosts(
                        _configuration.ActiveSite, postsString) as List<BooruImageModelBase>;

                    foreach (var booruImageModelBase in _currentPageBooruPosts)
                    {
                        var image = await _imageFetcherService.FetchImageAsync(
                            booruImageModelBase, ImageSizeType.Preview,
                            cancellationToken: cancellationToken,
                            caching: false);

                        cancellationToken.ThrowIfCancellationRequested();

                        var booruImageWrapper = new BooruImageWrapper
                        {
                            Hash = booruImageModelBase.Hash,
                            Image = image
                        };

                        NewBooruImages.Add(booruImageWrapper);
                    }

                    IsProcessing = false;
                }
                catch (HttpRequestException e)
                {
                    ToggleErrorOccured.Execute(new object());
                    ErrorInfo = e.Message;
                    IsProcessing = false;
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    ToggleErrorOccured.Execute(new object());
                    ErrorInfo = e.Message;
                    IsProcessing = false;
                }
            }));

        private RelayCommand _openFullImageCommand;
        public RelayCommand OpenFullCommand =>
            _openFullImageCommand ??
            (_openFullImageCommand = new RelayCommand(o =>
            {
                var openedBooruImageWrapper = o as BooruImageWrapper;
                _messenger.Publish(new OpenFullImageMessage(
                    this, openedBooruImageWrapper, _currentPageBooruPosts,
                    NewBooruImages, QueryPage));

                

                //_cancellationTokenSource = new CancellationTokenSource();
                //var boouru = o as BooruImageModelBase;
                //_messenger.Publish(new OpenFullImageMessage(this, o as BooruImageModelBase, BooruImages,
                //    PageNumberKeeper.NextQueriedPage - 1));
                //try
                //{
                //    await BooruWorker.LoadFullImage(boouru, _cancellationTokenSource.Token);
                //}
                //catch (HttpRequestException e)
                //{
                //    boouru.FullImage = null;
                //    _messenger.Publish(new CloseFullImageMessage(new object()));
                //    ToggleErrorOccured.Execute(new object());
                //    ErrorInfo = e.Message;
                //}
                //catch (Exception e)
                //{
                //    ToggleErrorOccured.Execute(new object());
                //    _messenger.Publish(new CloseFullImageMessage(new object()));
                //    ErrorInfo = e.Message;
                //}
            }));

        private RelayCommand _prevPageCommand;
        public RelayCommand PrevPageCommand => 
            _prevPageCommand ?? (
                _prevPageCommand = new RelayCommand(o =>
                {
                    if (_queryPage == 1) return;
                    _cancellationTokenSource.Cancel();
                    QueryPage--;
                    LoadCommand.Execute(null);
                    //if (PageNumberKeeper.DisplayedPage == 1) return;
                    //_cancellationTokenSource.Cancel();
                    //if (_pageNumberKeeper.QuriedPagesAccordance.ContainsKey(PageNumberKeeper.DisplayedPage - 1))
                    //{
                    //    PageNumberKeeper.NextQueriedPage =
                    //        PageNumberKeeper.QuriedPagesAccordance[PageNumberKeeper.DisplayedPage - 1];
                    //    PageNumberKeeper.QuriedPagesAccordance.Remove(PageNumberKeeper.DisplayedPage - 1);
                    //}
                    //else if (PageNumberKeeper.DisplayedPage - 1 == 1)
                    //{
                    //    PageNumberKeeper.ResetQueryInfo();
                    //} else
                    //{
                    //    PageNumberKeeper.NextQueriedPage = PageNumberKeeper.NextQueriedPage - PageNumberKeeper.QueriedPagesCount - 1;
                    //}
                    //PageNumberKeeper.DisplayedPage--;
                    //BooruImages.Clear();
                    //PageNumberKeeper.QueriedPagesCount = 0;
                    //_pageNumberKeeper.AddedImagesCount = 0;
                    //LoadCommand.Execute(null);
                }));

        private RelayCommand _nextPageCommand;
        public RelayCommand NextPageCommand =>
            _nextPageCommand ?? (
                _nextPageCommand = new RelayCommand( o => {
                    _cancellationTokenSource.Cancel();
                    QueryPage++;
                    LoadCommand.Execute(null);
                    //_cancellationTokenSource.Cancel();
                    //if (_pageNumberKeeper.QueriedPagesCount > 1)
                    //{
                    //    _pageNumberKeeper.QuriedPagesAccordance.Add(_pageNumberKeeper.DisplayedPage + 1,
                    //        _pageNumberKeeper.NextQueriedPage);
                    //}
                    //if (IsProcessing) _pageNumberKeeper.NextQueriedPage++;
                    //_pageNumberKeeper.DisplayedPage++;
                    //BooruImages.Clear();
                    //_pageNumberKeeper.QueriedPagesCount = 0;
                    //_pageNumberKeeper.AddedImagesCount = 0;
                    //LoadCommand.Execute(null);
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
            PageNumberKeeper.ResetAll();
        }

        private void DropImages(ResetBooruImagesMessage message)
        {
            _cancellationTokenSource?.Cancel();
            BooruImages = new ObservableCollection<BooruImageModelBase>();
            PageNumberKeeper.ResetAll();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
 
    }
}
