using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cardbooru.Application;
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
    class FullImageBrowsingViewModel : 
        IUserControlViewModel {

        private readonly IMvxMessenger _messenger;
        private readonly IImageFetcherService _imageFetcherService;
        private readonly IPostCollectionManager _postCollectionManager;
        private readonly IPostFetcherService _postFetcherService;
        private readonly IBooruConfiguration _configuration;
        private readonly IBooruPostManager _booruPostManager;
        private CancellationTokenSource _cancellationTokenSource;  
        private List<IBooruPost> _posts;
        private BooruImageWrapper _currentBooruImage;
        private List<BooruImageWrapper> _booruImages;
        private int _queryPage;
        private int _currentBooruImageIndex = -1;
        private BooruFullImageViewer _fullImageViewer;

        public event PropertyChangedEventHandler PropertyChanged;
        public List<string> TagsList { get; set; }
        public ImageSource Image { get; set; } 
        public bool IsFullImageLoaded { get; set; }

        public FullImageBrowsingViewModel(IMvxMessenger messenger,
            IImageFetcherService imageFetcherService,
            IPostCollectionManager postCollectionManager,
            IPostFetcherService postFetcherService,
            IBooruConfiguration configuration,
            IBooruPostManager booruPostManager,
            IBooruFullImageViewerFactory booruFullImageViewerFactory
            )
        {
            _messenger = messenger;
            _imageFetcherService = imageFetcherService;
            _postCollectionManager = postCollectionManager;
            _postFetcherService = postFetcherService;
            _configuration = configuration;
            _booruPostManager = booruPostManager;
            _fullImageViewer = booruFullImageViewerFactory.Create();
        }

        //Todo navigation param toggles nav buttons
        //ToDo Respect tags
        public void Init(
            BooruImageWrapper openedBooruImage,
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
                //Image = _currentBooruImage.Image;
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
                _messenger.Publish(new CloseFullImageMessage(this));
            }));
        }

        private RelayCommand _nextImage;
        public RelayCommand NextImage => _nextImage ?? (_nextImage = new RelayCommand(async o => {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            if (_currentBooruImageIndex == -1)
            {
                _currentBooruImageIndex = _posts.FindIndex(b => b.Hash == _currentBooruImage.Hash);
            }

            IsFullImageLoaded = false;

            if (_currentBooruImageIndex == _booruImages.Count - 1)
            {
                Image = null;
                var nextPostsString =
                    await _postFetcherService.FetchPostsAsync(_configuration.ActiveSite,
                        pageNumber: _queryPage + 1);

                var nextPosts =
                    _booruPostManager.DeserializePosts(_configuration.ActiveSite,
                        nextPostsString);

                var nextBooruImages =
                    await _postCollectionManager.GetImagesAsync(_configuration.ActiveSite, 
                        ImageSizeType.Preview, nextPosts);

                if (cancellationToken.IsCancellationRequested) return;

                _queryPage++;
                _currentBooruImageIndex = -1;
                _currentBooruImage = _booruImages[0];
                _booruImages = nextBooruImages;
                _posts = nextPosts;
            }
                                   
            Image = _booruImages[++_currentBooruImageIndex].Image;

            var nextBooruImage =
                _posts.FirstOrDefault(b => b.Hash == _booruImages[_currentBooruImageIndex].Hash);
            BitmapImage fullImage = null;
            try
            {
                fullImage = await _imageFetcherService.FetchImageAsync(nextBooruImage, ImageSizeType.Full,
                    cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                return;
            }

            if (cancellationToken.IsCancellationRequested == true)
            {
                _currentBooruImageIndex--;
                return;
            }
            Image = fullImage;
            IsFullImageLoaded = true;
            //if(_currentImageIndex == -1)
            //    await Task.Run(() => FindCurrentImageIndex());
            //if (_currentImageIndex + 1 == _booruImagesCollection.Count)
            //{
            //    _booruImagesCollection = new ObservableCollection<BooruPost>();

            //    await BooruWorker.FillBooruImages(new PageNumberKeeper{NextQueriedPage = ++_currentPage}, _booruImagesCollection,
            //        (BooruSiteType)Enum.Parse(typeof(BooruSiteType), Properties.Settings.Default.CurrentSite), cancellationTokenSource.Token);
            //    _currentImageIndex = -1;
            //}


            //var nextImage = _booruImagesCollection[++_currentImageIndex];
            //PreviewImage = nextImage.PreviewImage;
            //await BooruWorker.LoadFullImage(nextImage, cancellationTokenSource.Token);
            //BooruImageModel.FullImage = null;
            //BooruImageModel = nextImage;
            //TagsList = BooruImageModel.TagsList;
        }));

        private RelayCommand _prevImage;
        public RelayCommand PrevImage => _prevImage ?? (_prevImage = new RelayCommand(async o => {
            if (_currentBooruImageIndex == -1)
                _currentBooruImageIndex = _posts.FindIndex(b => b.Hash == _currentBooruImage.Hash);
            if (_queryPage == 1 && _currentBooruImageIndex == 0) return;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            IsFullImageLoaded = false;

            if (_currentBooruImageIndex == 0)
            {
                Image = null;
                var prevPosts =
                    await _postFetcherService.FetchPostsAsync(_configuration.ActiveSite,
                        pageNumber: _queryPage - 1);

                var prevBooruImageModelPosts =
                    _booruPostManager.DeserializePosts(_configuration.ActiveSite,
                        prevPosts);

                var prevBooruImageWrapperList =
                    await _postCollectionManager.GetImagesAsync(_configuration.ActiveSite,
                        ImageSizeType.Preview, prevBooruImageModelPosts);

                if (cancellationToken.IsCancellationRequested) return;

                _queryPage--;
                _currentBooruImageIndex = prevBooruImageWrapperList.Count - 1;
                _booruImages = prevBooruImageWrapperList;
                _currentBooruImage = _booruImages[_currentBooruImageIndex];
                _posts = prevBooruImageModelPosts;
                Image = _currentBooruImage.Image;

                BitmapImage fullImage = null;
                try
                {
                    fullImage = await _imageFetcherService.FetchImageAsync(prevBooruImageModelPosts[prevBooruImageModelPosts.FindIndex(
                            b => b.Hash == _currentBooruImage.Hash)], ImageSizeType.Full,
                        cancellationToken: cancellationToken);
                }
                catch (OperationCanceledException e)
                {
                    return;
                }

                if (cancellationToken.IsCancellationRequested) return;
                Image = fullImage;
                IsFullImageLoaded = true;
                return;
            }
                
            Image = _booruImages[--_currentBooruImageIndex].Image;

            var prevBooruImage =
                 _posts
                    .FirstOrDefault(b => b.Hash == _booruImages[_currentBooruImageIndex].Hash);
            BitmapImage prevFullImage = null;

            try
            {
                prevFullImage = await _imageFetcherService.FetchImageAsync(prevBooruImage, ImageSizeType.Full,
                    cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                return;
            }


            if (cancellationToken.IsCancellationRequested)
            {
                _currentBooruImageIndex++;
                return;
            }

            Image = prevFullImage;
            IsFullImageLoaded = true;
            //if (_currentPage == 1 && _currentImageIndex==0) return;
            //BooruImageModel.IsFullImageLoaded = false;
            //if (_currentImageIndex == -1)
            //    await Task.Run(() => FindCurrentImageIndex());
            //if (_currentImageIndex == 0)
            //{
            //    _booruImagesCollection = new ObservableCollection<BooruPost>();

            //    await BooruWorker.FillBooruImages(new PageNumberKeeper { NextQueriedPage = --_currentPage }, _booruImagesCollection,
            //        (BooruSiteType)Enum.Parse(typeof(BooruSiteType), Properties.Settings.Default.CurrentSite), cancellationTokenSource.Token);
            //    _currentImageIndex = _booruImagesCollection.Count;
            //}


            //var prevImage = _booruImagesCollection[--_currentImageIndex];
            //PreviewImage = prevImage.PreviewImage;
            //await BooruWorker.LoadFullImage(prevImage, cancellationTokenSource.Token);
            //BooruImageModel.FullImage = null;
            //BooruImageModel = prevImage;
            //TagsList = BooruImageModel.TagsList;
        }));

        #endregion

    }
}
