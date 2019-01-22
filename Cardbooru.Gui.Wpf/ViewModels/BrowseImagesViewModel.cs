using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Infrastructure.Messages;
using Cardbooru.Application.Interfaces;
using Cardbooru.Domain;
using Cardbooru.Domain.Entities;
using Cardbooru.Gui.Wpf.Infrastructure;
using Cardbooru.Gui.Wpf.Interfaces;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Gui.Wpf.ViewModels
{
    public class BrowseImagesViewModel : 
        IUserControlViewModel {

        private IDisposable _settingsUpdatedToken;
        private IDisposable _resetToken;
        private readonly IMvxMessenger _messenger;
        private readonly IPostCollectionManager _postCollectionManager;
        private readonly IImageFetcherService _imageFetcherService;
        private readonly IPostFetcherService _postFetcherService;
        private readonly IBooruConfiguration _configuration;
        private readonly IBooruPostManager _booruPostManager;
        private CancellationTokenSource _cancellationTokenSource;
        private List<IBooruPost> _currentPageBooruPosts;

        public event PropertyChangedEventHandler PropertyChanged;
        public int QueryPage { get; set; } = 1;
        /// <summary>
        /// Indicate whether something something doing a thing
        /// </summary>
        public bool IsProcessing { get; set; }
        public bool IsErrorOccured { get; set; }
        public string ErrorInfo { get; set; }
        public object CurrentScroll { get; set; }
        public ObservableCollection<BooruImageWrapper> BooruImages { get; set; } =
            new ObservableCollection<BooruImageWrapper>();

        public BrowseImagesViewModel(IMvxMessenger messenger,
            IPostCollectionManager postCollectionManager,
            IImageFetcherService imageFetcherService,
            IPostFetcherService postFetcherService,
            IBooruConfiguration configuration,
            IBooruPostManager booruPostManager)
        {
            _messenger = messenger;
            _imageFetcherService = imageFetcherService;
            _postCollectionManager = postCollectionManager;
            _postFetcherService = postFetcherService;
            _configuration = configuration;
            _booruPostManager = booruPostManager;
            _settingsUpdatedToken = _messenger.Subscribe<SettingsUpdatedMessage>(SettingsUpdated);
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
                BooruImages.Clear();
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;
                IsProcessing = true;
                try
                {
                    var tags = new List<string>();
                    tags.Add(_booruPostManager.GetRatingTagString(_configuration.ActiveSite,
                        _configuration.FetchConfiguration.RatingConfiguration));

                    var postsString = await _postFetcherService.FetchPostsAsync(_configuration.ActiveSite,
                        _configuration.FetchConfiguration.PostLimit, QueryPage, tags);

                    _currentPageBooruPosts =  _booruPostManager.DeserializePosts(
                        _configuration.ActiveSite, postsString);

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

                        BooruImages.Add(booruImageWrapper);
                    }

                    IsProcessing = false;
                }
                //catch (HttpRequestException e)
                //{
                //    ToggleErrorOccured.Execute(null);
                //    ErrorInfo = e.Message;
                //    IsProcessing = false;
                //}
                catch (OperationCanceledException) { }
                //catch (Exception e)
                //{
                //    ToggleErrorOccured.Execute(null);
                //    ErrorInfo = e.Message;
                //    IsProcessing = false;
                //}
            }));

        private RelayCommand _openFullImageCommand;
        public RelayCommand OpenFullCommand =>
            _openFullImageCommand ??
            (_openFullImageCommand = new RelayCommand(o =>
            {
                var openedBooruImageWrapper = o as BooruImageWrapper;
                _messenger.Publish(new OpenFullImageMessage(
                    this, openedBooruImageWrapper, _currentPageBooruPosts,
                    BooruImages, QueryPage));
            }));

        private RelayCommand _prevPageCommand;
        public RelayCommand PrevPageCommand => 
            _prevPageCommand ?? (
                _prevPageCommand = new RelayCommand(o =>
                {
                    if (QueryPage == 1) return;
                    _cancellationTokenSource.Cancel();
                    QueryPage--;
                    LoadCommand.Execute(null);
                }));

        private RelayCommand _nextPageCommand;
        public RelayCommand NextPageCommand =>
            _nextPageCommand ?? (
                _nextPageCommand = new RelayCommand( o => {
                    _cancellationTokenSource.Cancel();
                    QueryPage++;
                    LoadCommand.Execute(null);
                }));

        private RelayCommand _toggleErrorOccured;
        public RelayCommand ToggleErrorOccured => 
            _toggleErrorOccured ?? (
                _toggleErrorOccured = new RelayCommand(o => {
            IsErrorOccured = !IsErrorOccured;
            ErrorInfo = string.Empty;
        }));

        private RelayCommand _loadStateCommand;
        public RelayCommand LoadStateCommand => 
            _loadStateCommand ?? (
                _loadStateCommand = new RelayCommand(o => { }));
        #endregion

        private void SettingsUpdated(SettingsUpdatedMessage message) {
            _cancellationTokenSource?.Cancel();
            BooruImages.Clear();
        }

        private void DropImages(ResetBooruImagesMessage message)
        {
            _cancellationTokenSource?.Cancel();
            BooruImages.Clear();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
 
    }
}
