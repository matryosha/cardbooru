using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Data;
using Cardbooru.Application;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Infrastructure.Messages;
using Cardbooru.Application.Interfaces;
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
        private readonly BooruPostsProvider _booruPostsProvider;
        private readonly object _booruImagesLockObj = new object();
        private CancellationTokenSource _cancellationTokenSource;

        public event PropertyChangedEventHandler PropertyChanged;
        public int QueryPage { get; set; } = 1;
        /// <summary>
        /// Indicate whether something something doing a thing
        /// </summary>
        public bool IsProcessing { get; set; }
        public bool IsErrorOccured { get; set; }
        public string ErrorInfo { get; set; }
        public object CurrentScroll { get; set; }      
        public ObservableCollection<BooruImage> BooruImages { get; set; } = 
            new ObservableCollection<BooruImage>();

        public BrowseImagesViewModel(IMvxMessenger messenger,
            IBooruPostsProviderFactory postsProviderFactory)
        {
            _messenger = messenger;
            _settingsUpdatedToken = _messenger.Subscribe<SettingsUpdatedMessage>(SettingsUpdated);
            _resetToken = _messenger.Subscribe<ResetBooruImagesMessage>(DropImages);
            _booruPostsProvider = postsProviderFactory.Create();

            BindingOperations.EnableCollectionSynchronization(BooruImages, _booruImagesLockObj);
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
                    await _booruPostsProvider.GetPosts(AddImage, QueryPage, cancellationToken);

                    IsProcessing = false;
                }
                catch (HttpRequestException e)
                {
                    ToggleErrorOccured.Execute(null);
                    ErrorInfo = e.Message;
                    IsProcessing = false;
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    ToggleErrorOccured.Execute(null);
                    ErrorInfo = e.Message;
                    IsProcessing = false;
                }
            }));

        private RelayCommand _openFullImageCommand;
        public RelayCommand OpenFullCommand =>
            _openFullImageCommand ??
            (_openFullImageCommand = new RelayCommand(o =>
            {
                var openedBooruImage = o as BooruImage;
                _messenger.Publish(
                    new OpenFullImageMessage(this,
                        openedBooruImage, _booruPostsProvider));
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

        private void AddImage(BooruImage wrapper)
        {
            lock (_booruImagesLockObj)
            {
                BooruImages.Add(wrapper);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
 
    }
}
