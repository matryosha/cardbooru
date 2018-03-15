using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Cardbooru.Helpers;
using Cardbooru.Helpers.Base;
using Cardbooru.Models.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.FullImageBrowsing
{
    class FullImageBrowsingViewModel : 
        INotifyPropertyChanged, IUserControlViewModel {

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

        private ObservableCollection<BooruImageModelBase> _booruImagesCollection;

        public FullImageBrowsingViewModel(BooruImageModelBase booruImageModel) {
            Messenger = IdkInjection.MessengerHub;
            BooruImageModel = booruImageModel;
            TagsList = booruImageModel.TagsList;
        }

        public FullImageBrowsingViewModel(BooruImageModelBase booruImageModel, ObservableCollection<BooruImageModelBase> collection = null) {
            Messenger = IdkInjection.MessengerHub;
            BooruImageModel = booruImageModel;
            _booruImagesCollection = collection;
            TagsList = booruImageModel.TagsList;
        }

        private RelayCommand _closeImageCommand;

        public RelayCommand CloseImageCommand {
            get => _closeImageCommand ?? (_closeImageCommand = new RelayCommand(o => {
                BooruImageModel.FullImage = null;
                BooruImageModel.IsFullImageLoaded = false;
                Messenger.Publish(new CloseFullImageMessage(this));
                }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
