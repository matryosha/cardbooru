﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Cardbooru.Helpers;
using Cardbooru.Helpers.Base;
using Cardbooru.Models.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.FullImageBrowsing
{
    class FullImageBrowsingViewModel : 
        INotifyPropertyChanged, IUserControlViewModel {

        private ObservableCollection<BooruImageModelBase> _booruImagesCollection;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private int _currentImageIndex = -1;
        private int _currentPage;
        private IMvxMessenger _messenger;

        public event PropertyChangedEventHandler PropertyChanged;

        private List<string> _tagsList;
        public List<string> TagsList {
            get => _tagsList;
            set {
                _tagsList = value;
                OnPropertyChanged("TagsList");
            }
        }

        private ImageSource _fullImage;
        public ImageSource FullImage {
            get => BooruImageModel.FullImage;
            set {
                _fullImage = value;
                OnPropertyChanged("FullImage");
            }
        }

        private ImageSource _previewImage;
        public ImageSource PreviewImage {
            get => _previewImage ?? (_previewImage = BooruImageModel.PreviewImage);
            set {
                _previewImage = value;
                OnPropertyChanged("PreviewImage");
            }
        }

        private BooruImageModelBase _booruImageModel;
        public BooruImageModelBase BooruImageModel {
            get => _booruImageModel;
            set {
                _booruImageModel = value;
                OnPropertyChanged("BooruImageModel");
            } 
        }

        private RelayCommand _closeImageCommand;
        public RelayCommand CloseImageCommand {
            get => _closeImageCommand ?? (_closeImageCommand = new RelayCommand(o => {
                BooruImageModel.FullImage = null;
                BooruImageModel.IsFullImageLoaded = false;
                _messenger.Publish(new CloseFullImageMessage(this));
                }));
        }

        private RelayCommand _nextImage;
        public  RelayCommand NextImage {
            get => _nextImage ?? (_nextImage = new RelayCommand( async o => {
                //TODO anti fast click
                BooruImageModel.IsFullImageLoaded = false;
                if(_currentImageIndex == -1)
                    await Task.Run(() => FindCurrentImageIndex());
                if (_currentImageIndex + 1 == _booruImagesCollection.Count)
                {
                    _booruImagesCollection = new ObservableCollection<BooruImageModelBase>();

                    await BooruWorker.FillBooruImages(new PageNumberKeeper{NextQueriedPage = ++_currentPage}, _booruImagesCollection,
                        (BooruSiteType)Enum.Parse(typeof(BooruSiteType), Properties.Settings.Default.CurrentSite), cancellationTokenSource.Token);
                    _currentImageIndex = -1;
                }
                   

                var nextImage = _booruImagesCollection[++_currentImageIndex];
                PreviewImage = nextImage.PreviewImage;
                await BooruWorker.LoadFullImage(nextImage, cancellationTokenSource.Token);
                BooruImageModel.FullImage = null;
                BooruImageModel = nextImage;
                TagsList = BooruImageModel.TagsList;
                }));
        }

        private RelayCommand _prevImage;
        public RelayCommand PrevImage {
            get => _prevImage ?? (_prevImage = new RelayCommand(async o => {
                //TODO anti fast click
                if (_currentPage == 1 && _currentImageIndex==0) return;
                BooruImageModel.IsFullImageLoaded = false;
                if (_currentImageIndex == -1)
                    await Task.Run(() => FindCurrentImageIndex());
                if (_currentImageIndex == 0)
                {
                    _booruImagesCollection = new ObservableCollection<BooruImageModelBase>();

                    await BooruWorker.FillBooruImages(new PageNumberKeeper { NextQueriedPage = --_currentPage }, _booruImagesCollection,
                        (BooruSiteType)Enum.Parse(typeof(BooruSiteType), Properties.Settings.Default.CurrentSite), cancellationTokenSource.Token);
                    _currentImageIndex = _booruImagesCollection.Count;
                }
                    

                var prevImage = _booruImagesCollection[--_currentImageIndex];
                PreviewImage = prevImage.PreviewImage;
                await BooruWorker.LoadFullImage(prevImage, cancellationTokenSource.Token);
                BooruImageModel.FullImage = null;
                BooruImageModel = prevImage;
                TagsList = BooruImageModel.TagsList;
            }));
        }

        public FullImageBrowsingViewModel(BooruImageModelBase booruImageModel, IMvxMessenger messenger) {
            _messenger = messenger;
            BooruImageModel = booruImageModel;
            TagsList = booruImageModel.TagsList;
        }

        public FullImageBrowsingViewModel(BooruImageModelBase booruImageModel, IMvxMessenger messenger, ObservableCollection<BooruImageModelBase> collection, int page) {
            _messenger = messenger;
            BooruImageModel = booruImageModel;
            _booruImagesCollection = collection;
            TagsList = booruImageModel.TagsList;
            _currentPage = page;
        }

        private void FindCurrentImageIndex() {
            _currentImageIndex = _booruImagesCollection.IndexOf(BooruImageModel);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
