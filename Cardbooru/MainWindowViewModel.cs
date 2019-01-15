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
using MvvmCross.Plugins.Messenger;
using Ninject;

namespace Cardbooru
{
    public class MainWindowViewModel : INotifyPropertyChanged {

        private IUserControlViewModel _currentView;
        private IDisposable _tokenFromBrowseImage;
        private IDisposable _tokenFromFullImageBrowse;
        private List<IUserControlViewModel> _viewModels;
        private IMvxMessenger _messenger;

        public List<IUserControlViewModel> ViewModels => _viewModels ?? (_viewModels = new List<IUserControlViewModel>());

        public IUserControlViewModel CurrentView {
            get => _currentView;
            set {
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }

        public MainWindowViewModel(IMvxMessenger messenger, IKernel iocKernel) {
            _messenger = messenger;
            CurrentView = iocKernel.Get<BrowseImagesViewModel>();
            ViewModels.Add(CurrentView);
            ViewModels.Add(iocKernel.Get<SettingsViewModel>());
            _tokenFromBrowseImage = _messenger.Subscribe<OpenFullImageMessage>(ShowFullImage);
            _tokenFromFullImageBrowse = _messenger.Subscribe<CloseFullImageMessage>(ChangeViewToBrowseImage);
        }

        private void ShowFullImage(OpenFullImageMessage fullImage) {
            var fullImageView = new FullImageBrowsingViewModel(fullImage.BooruImageModel, _messenger, fullImage.BooruImageCollection, fullImage.CurrentPage);
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
                CurrentView = ViewModels[1];
            }));

        private RelayCommand _openBrowsingWindowCommand;

        public RelayCommand OpenBrowsingWindowCommand => _openBrowsingWindowCommand ?? (
                                                             _openBrowsingWindowCommand = new RelayCommand(o => {
                                                                 ChangeViewToBrowseImage(
                                                                     new CloseFullImageMessage(new object()));
                                                             }));

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
