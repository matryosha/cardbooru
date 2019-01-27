using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Cardbooru.Application;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Exceptions;
using Cardbooru.Application.Infrastructure.Messages;
using Cardbooru.Application.Interfaces;
using Cardbooru.Gui.Wpf.Infrastructure;
using Cardbooru.Gui.Wpf.Interfaces;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.Gui.Wpf.ViewModels
{
    class FullImageBrowsingViewModel : 
        IUserControlViewModel {

        private readonly IMvxMessenger _messenger;
        private CancellationTokenSource _cancellationTokenSource;  
        private BooruImage _currentBooruImage;
        private BooruFullImageViewer _fullImageViewer;

        public event PropertyChangedEventHandler PropertyChanged;
        public List<string> TagsList { get; set; }
        public ImageSource Image { get; set; } 
        public bool IsFullImageLoaded { get; set; }

        public FullImageBrowsingViewModel(IMvxMessenger messenger,
            IBooruFullImageViewerFactory booruFullImageViewerFactory
            )
        {
            _messenger = messenger;
            _fullImageViewer = booruFullImageViewerFactory.Create();
        }

        //Todo navigation param toggles nav buttons
        //ToDo Respect tags
        public void Init(
            BooruImage openedBooruImage,
            BooruPostsProvider provider)
        {
            _fullImageViewer.Init(provider, openedBooruImage);
            _currentBooruImage = openedBooruImage;
            Image = openedBooruImage.Image;
        }

        public async Task ShowFullImage()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;
                //Todo sometimes pic does not load
                Image =
                    await _fullImageViewer.FetchImageAsync(_currentBooruImage, cancellationToken);
                IsFullImageLoaded = true;
            }
            catch (OperationCanceledException e)
            {

            }
        }

        #region Commands
        private RelayCommand _closeImageCommand;
        public RelayCommand CloseImageCommand
        {
            get => _closeImageCommand ?? (_closeImageCommand = new RelayCommand(o =>
            {
                //BooruImageModel.FullImage = null;
                //BooruImageModel.IsFullImageLoaded = false;
                _fullImageViewer = null;
                _messenger.Publish(new CloseFullImageMessage(this));
            }));
        }

        private RelayCommand _nextImage;
        public RelayCommand NextImage => _nextImage ?? (_nextImage = new RelayCommand(async o => {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            IsFullImageLoaded = false;  
            
            BooruImage booruImage;
            try
            {
                booruImage = await _fullImageViewer.GetNextBooruImageAsync(SetImage, cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                return;
            }
            Image = booruImage.Image;
 
            IsFullImageLoaded = true;
        }));

        private RelayCommand _prevImage;
        public RelayCommand PrevImage => _prevImage ?? (_prevImage = new RelayCommand(async o => {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            IsFullImageLoaded = false;

            BooruImage booruImage;
            try
            {
                booruImage = await _fullImageViewer.GetPrevBooruImageAsync(SetImage, cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                return;
            }
            catch (QueryPageException e)
            {
                IsFullImageLoaded = true;
                return;
            }
            Image = booruImage.Image;

            IsFullImageLoaded = true;
        }));

        #endregion

        private void SetImage(BooruImage booruImage)
        {
            _currentBooruImage = booruImage;
            Image = booruImage.Image;
        }

    }
}
