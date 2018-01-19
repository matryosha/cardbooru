using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cardbooru.Helpers;
using Cardbooru.Helpers.Base;
using Cardbooru;
using Cardbooru.Models.Base;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru.BrowseImages
{
    public class BrowseImagesViewModel : 
        INotifyPropertyChanged, IUserControlViewModel {

        private int _currentPage;
        /// <summary>
        /// Indicate whether pictures loading to list or not
        /// </summary>
        private bool _isLoadling;
        private OpenFullImageMessage _openFullImageMessage;

        public object CurrentOpenedItemState { get; private set; }

        private BooruWorker booruWorker; // TODO Make Interface and make static?

        public IMvxMessenger Messenger { get; }
        public ObservableCollection<BooruImageModelBase> BooruImages { get; set; } = 
            new ObservableCollection<BooruImageModelBase>();
        

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
                await booruWorker.FillBooruImages(_currentPage, BooruImages, BooruType.Danbooru);
                _currentPage++;
                _isLoadling = false;
            }));

        private RelayCommand _openFullImageCommand;

        public RelayCommand OpenFullCommnad => _openFullImageCommand ??
                                               (_openFullImageCommand = new RelayCommand(async o => {
                                                   var boouru = o as BooruImageModelBase;
                                                   //call method to draw loading image
                                                   await booruWorker.LoadFullImage(boouru);
                                                   _openFullImageMessage = new OpenFullImageMessage(this, o as BooruImageModelBase);
                                                   Messenger.Publish(_openFullImageMessage);
                                               }));


        private RelayCommand _saveStateCommand;

        public RelayCommand SaveStateCommand => _saveStateCommand ??
                                                (_saveStateCommand = new RelayCommand(openedListItem => {
                                                    CurrentOpenedItemState = openedListItem;
                                                }));

        private RelayCommand _loadStateCommand;
        public RelayCommand LoadStateCommand => _loadStateCommand ?? (_loadStateCommand = new RelayCommand(o => { }));
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        
    }
}
