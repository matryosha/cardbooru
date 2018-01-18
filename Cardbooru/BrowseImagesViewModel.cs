using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cardbooru.Helpers.Base;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru
{
    public class BrowseImagesViewModel : 
        INotifyPropertyChanged, IUserControlViewModel {
        private int _currentPage;
        private bool _isLoadling;
        private OpenFullImageMessage _openFullImageMessage;

        private BooruWorker booruWorker; // TODO Make Interface and make static?

        public IMvxMessenger Messenger { get; }
        public ObservableCollection<BooruImage> BooruImages { get; set; } = 
            new ObservableCollection<BooruImage>();
        

        public BrowseImagesViewModel() {
            Messenger = IdkInjection.MessengerHub;
            _currentPage = 1;
            booruWorker = new BooruWorker();
        }


        #region Commands

        private RelayCommand _loadPreviewImages;
        public RelayCommand LoadCommand => _loadPreviewImages ?? 
            (_loadPreviewImages = new RelayCommand(async o => {
                if(_isLoadling) return;
                _isLoadling = true;
                await booruWorker.FillBooruImages(_currentPage, BooruImages);
                _currentPage++;
                _isLoadling = false;
            }));

        private RelayCommand _openFullImage;

        public RelayCommand OpenFullCommnad => _openFullImage ??
                                               (_loadPreviewImages = new RelayCommand(async o => {
                                                   var boouru = o as BooruImage;
                                                   await booruWorker.LoadFullImage(boouru);
                                                   _openFullImageMessage = new OpenFullImageMessage(this, o as BooruImage);
                                                   Messenger.Publish(_openFullImageMessage);
                                               }));

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
