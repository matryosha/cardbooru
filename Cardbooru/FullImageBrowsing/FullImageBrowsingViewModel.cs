using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cardbooru.Application.Configurations;
using Cardbooru.Application.Entities;
using Cardbooru.Application.Interfaces;
using Cardbooru.Helpers;
using Cardbooru.Helpers.Base;
using Cardbooru.Models.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.FullImageBrowsing
{
    class FullImageBrowsingViewModel : 
        IUserControlViewModel {

        private readonly IMvxMessenger _messenger;
        private readonly IImageFetcherService _imageFetcherService;
        private readonly IPostCollectionManager _postCollectionManager;
        private readonly IPostFetcherService _postFetcherService;
        private readonly RootConfiguration _configuration;
        private CancellationTokenSource _cancellationTokenSource;  
        private List<BooruImageModelBase> _booruImageModelPosts;
        private BooruImageWrapper _booruImageWrapper;
        private List<BooruImageWrapper> _booruImageWrapperList;
        private int _queryPage;
        private int _lastImageIndex = -1;

        public event PropertyChangedEventHandler PropertyChanged;
        public List<string> TagsList { get; set; }
        public ImageSource Image { get; set; } 
        public bool IsFullImageLoaded { get; set; }

        public FullImageBrowsingViewModel(IMvxMessenger messenger,
            IImageFetcherService imageFetcherService,
            IPostCollectionManager postCollectionManager,
            IPostFetcherService postFetcherService,
            RootConfiguration configuration)
        {
            _messenger = messenger;
            _imageFetcherService = imageFetcherService;
            _postCollectionManager = postCollectionManager;
            _postFetcherService = postFetcherService;
            _configuration = configuration;
        }

        //Todo navigation param toggles nav buttons
        public void Init(
            BooruImageWrapper booruImageWrapper,
            List<BooruImageModelBase> posts)
        {
            _booruImageModelPosts = posts;
            _booruImageWrapper = booruImageWrapper;
        }

        public void Init(
            BooruImageWrapper booruImageWrapper,
            List<BooruImageModelBase> posts,
            ICollection<BooruImageWrapper> booruImageWrapperList,
            int queryPage)
        {
            _booruImageModelPosts = posts;
            _booruImageWrapper = booruImageWrapper;
            _booruImageWrapperList = new List<BooruImageWrapper>(booruImageWrapperList);
            _queryPage = queryPage;
        }

        public async Task ShowFullImage()
        {
            Image = _booruImageWrapper.Image;
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            var booruImageModel = _booruImageModelPosts
                .FirstOrDefault(b => b.Hash == _booruImageWrapper.Hash);
            //Todo sometimes pic does not load
            var fullImage = await _imageFetcherService.FetchImageAsync(
                booruImageModel, ImageSizeType.Full);

            Image = fullImage;
            IsFullImageLoaded = true;
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
        public RelayCommand NextImage
        {
            get => _nextImage ?? (_nextImage = new RelayCommand(async o => {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;

                if (_lastImageIndex == -1)
                {
                    _lastImageIndex = _booruImageModelPosts.FindIndex(b => b.Hash == _booruImageWrapper.Hash);
                }

                IsFullImageLoaded = false;

                if (_lastImageIndex == _booruImageWrapperList.Count - 1)
                {
                    Image = null;
                    var nextPosts =
                        await _postFetcherService.FetchPostsAsync(_configuration.ActiveSite,
                            pageNumber: _queryPage + 1);

                    var nextBooruImageModelPosts =
                        _postCollectionManager.DeserializePosts(_configuration.ActiveSite,
                            nextPosts);

                    var nextBooruImageWrapperList =
                         await _postCollectionManager.GetImagesAsync(_configuration.ActiveSite, 
                             ImageSizeType.Preview, nextBooruImageModelPosts);

                    if (cancellationToken.IsCancellationRequested) return;



                    _queryPage++;
                    _lastImageIndex = -1;
                    _booruImageWrapper = _booruImageWrapperList[0];
                    _booruImageWrapperList = nextBooruImageWrapperList;
                    _booruImageModelPosts = nextBooruImageModelPosts;
                    //Image = _booruImageWrapper.Image;

                    //BitmapImage nextFullImage = null;
                    //try
                    //{
                    //    nextFullImage = await _imageFetcherService.FetchImageAsync(nextBooruImageModelPosts[0], ImageSizeType.Full,
                    //        cancellationToken: cancellationToken);
                    //}
                    //catch (OperationCanceledException e)
                    //{
                    //    return;
                    //}

                    //if (cancellationToken.IsCancellationRequested) return;
                    //Image = nextFullImage;
                    //IsFullImageLoaded = true;
                    //return;
                }
                                   
                Image = _booruImageWrapperList[++_lastImageIndex].Image;

                var nextBooruImageModel =
                    _booruImageModelPosts.FirstOrDefault(b => b.Hash == _booruImageWrapperList[_lastImageIndex].Hash);
                BitmapImage fullImage = null;
                try
                {
                    fullImage = await _imageFetcherService.FetchImageAsync(nextBooruImageModel, ImageSizeType.Full,
                        cancellationToken: cancellationToken);
                }
                catch (AggregateException e)
                {
                    return;
                }
                catch (OperationCanceledException e)
                {
                    return;
                }

                if (cancellationToken.IsCancellationRequested == true)
                {
                    _lastImageIndex--;
                    return;
                }
                Image = fullImage;
                IsFullImageLoaded = true;
                //if(_currentImageIndex == -1)
                //    await Task.Run(() => FindCurrentImageIndex());
                //if (_currentImageIndex + 1 == _booruImagesCollection.Count)
                //{
                //    _booruImagesCollection = new ObservableCollection<BooruImageModelBase>();

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
        }

        private RelayCommand _prevImage;
        public RelayCommand PrevImage => _prevImage ?? (_prevImage = new RelayCommand(async o => {
            if (_lastImageIndex == -1)
                _lastImageIndex = _booruImageModelPosts.FindIndex(b => b.Hash == _booruImageWrapper.Hash);
            if (_queryPage == 1 && _lastImageIndex == 0) return;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            IsFullImageLoaded = false;

            if (_lastImageIndex == 0)
            {
                Image = null;
                var prevPosts =
                    await _postFetcherService.FetchPostsAsync(_configuration.ActiveSite,
                        pageNumber: _queryPage - 1);

                var prevBooruImageModelPosts =
                    _postCollectionManager.DeserializePosts(_configuration.ActiveSite,
                        prevPosts);

                var prevBooruImageWrapperList =
                    await _postCollectionManager.GetImagesAsync(_configuration.ActiveSite,
                        ImageSizeType.Preview, prevBooruImageModelPosts);

                if (cancellationToken.IsCancellationRequested) return;

                _queryPage--;
                _lastImageIndex = prevBooruImageWrapperList.Count - 1;
                _booruImageWrapperList = prevBooruImageWrapperList;
                _booruImageWrapper = _booruImageWrapperList[_lastImageIndex];
                _booruImageModelPosts = prevBooruImageModelPosts;
                Image = _booruImageWrapper.Image;

                BitmapImage fullImage = null;
                try
                {
                    fullImage = await _imageFetcherService.FetchImageAsync(prevBooruImageModelPosts[prevBooruImageModelPosts.FindIndex(
                            b => b.Hash == _booruImageWrapper.Hash)], ImageSizeType.Full,
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
                
            Image = _booruImageWrapperList[_lastImageIndex - 1].Image;

            var prevBooruImageModel =
                 _booruImageModelPosts
                    .FirstOrDefault(b => b.Hash == _booruImageWrapperList[_lastImageIndex - 1].Hash);
            BitmapImage prevFullImage = null;

            try
            {
                prevFullImage = await _imageFetcherService.FetchImageAsync(prevBooruImageModel, ImageSizeType.Full,
                    cancellationToken: cancellationToken);
            }
            catch (TaskCanceledException e)
            {
                return;
            }
            catch (OperationCanceledException e)
            {
                return;
            }
            

            if(cancellationToken.IsCancellationRequested) return;

            Image = prevFullImage;
            _lastImageIndex--;
            IsFullImageLoaded = true;
            //if (_currentPage == 1 && _currentImageIndex==0) return;
            //BooruImageModel.IsFullImageLoaded = false;
            //if (_currentImageIndex == -1)
            //    await Task.Run(() => FindCurrentImageIndex());
            //if (_currentImageIndex == 0)
            //{
            //    _booruImagesCollection = new ObservableCollection<BooruImageModelBase>();

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
