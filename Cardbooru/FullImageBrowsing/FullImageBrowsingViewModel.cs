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

        public event PropertyChangedEventHandler PropertyChanged;
        public IMvxMessenger Messenger { get; }
        public List<string> TagsList { get; set; }

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
                BooruImageModel.IsFullImageLoaded = false;
                if(_currentImageIndex == -1)
                    await Task.Run(() => FindCurrentImageIndex());
                if(_currentImageIndex >= _booruImagesCollection.Count) throw new Exception("Not Implement yet");
                
                BooruImageModel = _booruImagesCollection[++_currentImageIndex];
            }));
        }

        private RelayCommand _prevImage;
        public RelayCommand PrevImage {
            get => _prevImage ?? (_prevImage = new RelayCommand(o => { }));
        }


        private ObservableCollection<BooruImageModelBase> _booruImagesCollection;
        private int _currentImageIndex = -1;
        private int _currentPage;

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
