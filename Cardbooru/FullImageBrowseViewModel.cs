using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Cardbooru.Helpers.Base;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru
{
    class FullImageBrowseViewModel : 
        INotifyPropertyChanged, IUserControlViewModel {

        public IMvxMessenger Messenger { get; }

        private ImageSource _fullImage;
        public ImageSource FullImage {
            get => _fullImage;
            set {
                _fullImage = value;
                OnPropertyChanged("FullImage");
            }
        }

        public FullImageBrowseViewModel(ImageSource image) {
            Messenger = IdkInjection.MessengerHub;
            FullImage = image;
        }

        private RelayCommand _closeImageCommand;

        public RelayCommand CloseImageCommand {
            get => _closeImageCommand ?? (_closeImageCommand = new RelayCommand(o => {
                _fullImage = null;

            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
