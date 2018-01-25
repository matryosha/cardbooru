using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Cardbooru.BrowseImages;
using Cardbooru.FullImageBrowsing;
using Cardbooru.Helpers;
using Cardbooru.Helpers.Base;
using Cardbooru.Settings;

namespace Cardbooru
{
    public class AppModelView : INotifyPropertyChanged {

        private IUserControlViewModel _currentView;
        private IDisposable _tokenFromBrowseImage;
        private IDisposable _tokenFromFullImageBrowse;
        private List<IUserControlViewModel> _viewModels;

        public List<IUserControlViewModel> ViewModels => _viewModels ?? (_viewModels = new List<IUserControlViewModel>());

        public IUserControlViewModel CurrentView {
            get => _currentView;
            set {
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }

        public AppModelView() {
            var messenger = IdkInjection.MessengerHub;
            _tokenFromBrowseImage = messenger.Subscribe<OpenFullImageMessage>(ShowFullImage);
            _tokenFromFullImageBrowse = messenger.Subscribe<CloseFullImageMessage>(ChangeViewToBrowseImage);
            CurrentView = new BrowseImagesViewModel();
            ViewModels.Add(CurrentView);
        }

        private void ShowFullImage(OpenFullImageMessage fullImage) {
            var fullImageView = new FullImageBrowsingViewModel(fullImage.BooruImageModel);
            CurrentView = fullImageView;
        }

        private void ChangeViewToBrowseImage(CloseFullImageMessage message) {
            CurrentView = ViewModels[0];    
        }

        private void ChangeView(IUserControlViewModel viewModel) {
            if(!ViewModels.Contains(viewModel))
                ViewModels.Add(viewModel);

            CurrentView = ViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        private RelayCommand _openSettingsCommand;

        public RelayCommand OpenSettingsCommand => _openSettingsCommand ?? (
            _openSettingsCommand = new RelayCommand(o => {
                CurrentView = new SettingsViewModel();
            }));

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
