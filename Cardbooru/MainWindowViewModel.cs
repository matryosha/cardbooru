using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cardbooru.Helpers.Base;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru
{
    public class MainWindowViewModel : INotifyPropertyChanged {
        private int _currentPage;
        private bool _isLoadling;
        private OpenFullImageMessage _openFullImageMessage;
        private IMvxMessenger _messenger;

        private BooruWorker booruWorker; // TODO Make Interface and make static?

        public ObservableCollection<BooruImage> BooruImages { get; set; } = 
            new ObservableCollection<BooruImage>();
        

        public MainWindowViewModel() {
            _messenger = Mvx.Resolve<IMvxMessenger>();
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
                                                   _openFullImageMessage = new OpenFullImageMessage(this, o as BooruImage);
                                                   _messenger.Publish(_openFullImageMessage);
                                               }));
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
