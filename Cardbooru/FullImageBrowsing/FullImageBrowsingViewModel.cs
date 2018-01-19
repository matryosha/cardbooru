using System.Collections.Generic;
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
            get => _booruImageModel.FullImage.Source;
            set {
                _fullImage = value;
                OnPropertyChanged("FullImage");
            }
        }

        private BooruImageModelBase _booruImageModel;

        public FullImageBrowsingViewModel(BooruImageModelBase booruImageModel) {
            Messenger = IdkInjection.MessengerHub;
            _booruImageModel = booruImageModel;
            TagsList = booruImageModel.TagsList;
        }

        private RelayCommand _closeImageCommand;

        public RelayCommand CloseImageCommand {
            get => _closeImageCommand ?? (_closeImageCommand = new RelayCommand(o => {
                _booruImageModel.FullImage = null;
                Messenger.Publish(new CloseFullImageMessage(this));
                }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
