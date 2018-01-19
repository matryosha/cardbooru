using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Cardbooru.Helpers.Base;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru
{
    class FullImageBrowsingViewModel : 
        INotifyPropertyChanged, IUserControlViewModel {

        public IMvxMessenger Messenger { get; }

        private ImageSource _fullImage;
        public ImageSource FullImage {
            get => _booruImageModel.FullImage.Source;
            set {
                _fullImage = value;
                OnPropertyChanged("FullImage");
            }
        }

        private BooruImageModel _booruImageModel;

        public FullImageBrowsingViewModel(BooruImageModel booruImageModel) {
            Messenger = IdkInjection.MessengerHub;
            _booruImageModel = booruImageModel;
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
