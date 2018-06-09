using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        private int _currentImageIndex = -1;
        private int _currentPage;

        public event PropertyChangedEventHandler PropertyChanged;
        public IMvxMessenger Messenger { get; }

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
            get => BooruImageModel.FullImage.Source;
            set {
                _fullImage = value;
                OnPropertyChanged("FullImage");
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
                Messenger.Publish(new CloseFullImageMessage(this));
                }));
        }

        private RelayCommand _nextImage;
        public  RelayCommand NextImage {
            get => _nextImage ?? (_nextImage = new RelayCommand( async o => {
                //TODO anti fast click
                BooruImageModel.IsFullImageLoaded = false;
                if(_currentImageIndex == -1)
                    await Task.Run(() => FindCurrentImageIndex());
                if (_currentImageIndex+1 == _booruImagesCollection.Count)
                    await BooruWorker.FillBooruImages(++_currentPage, _booruImagesCollection,
                        (BooruType) Enum.Parse(typeof(BooruType), Properties.Settings.Default.CurrentSite));

                var nextImage = _booruImagesCollection[++_currentImageIndex];

                await BooruWorker.LoadFullImage(nextImage);
                BooruImageModel.FullImage = null;
                BooruImageModel = nextImage;
                TagsList = BooruImageModel.TagsList;
                }));
        }

        private RelayCommand _prevImage;
        public RelayCommand PrevImage {
            get => _prevImage ?? (_prevImage = new RelayCommand(async o => {
                //TODO anti fast click
                BooruImageModel.IsFullImageLoaded = false;
                if (_currentImageIndex == -1)
                    await Task.Run(() => FindCurrentImageIndex());
                if (_currentImageIndex == 0)
                    _currentImageIndex = _booruImagesCollection.Count;

                var prevImage = _booruImagesCollection[--_currentImageIndex];

                await BooruWorker.LoadFullImage(prevImage);
                BooruImageModel.FullImage = null;
                BooruImageModel = prevImage;
                TagsList = BooruImageModel.TagsList;
            }));
        }

        public FullImageBrowsingViewModel(BooruImageModelBase booruImageModel) {
            Messenger = IdkInjection.MessengerHub;
            BooruImageModel = booruImageModel;
            TagsList = booruImageModel.TagsList;
        }

        public FullImageBrowsingViewModel(BooruImageModelBase booruImageModel, ObservableCollection<BooruImageModelBase> collection, int page) {
            Messenger = IdkInjection.MessengerHub;
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
