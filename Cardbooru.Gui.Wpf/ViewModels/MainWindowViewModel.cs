using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Cardbooru.Application.Infrastructure.Messages;
using Cardbooru.Gui.Wpf.Infrastructure;
using Cardbooru.Gui.Wpf.Interfaces;
using MvvmCross.Plugins.Messenger;
using Ninject;

namespace Cardbooru.Gui.Wpf.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged {

        private IUserControlViewModel _currentView;
        private IDisposable _tokenFromBrowseImage;
        private IDisposable _tokenFromFullImageBrowse;
        private List<IUserControlViewModel> _viewModels;
        private IMvxMessenger _messenger;
        private readonly IKernel _iocKernel;

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
            _iocKernel = iocKernel;
            CurrentView = _iocKernel.Get<BrowseImagesViewModel>();
            ViewModels.Add(CurrentView);
            ViewModels.Add(_iocKernel.Get<SettingsViewModel>());
            _tokenFromBrowseImage = _messenger.Subscribe<OpenFullImageMessage>(OpenFullImage);
            _tokenFromFullImageBrowse = _messenger.Subscribe<CloseFullImageMessage>(ChangeViewToBrowseImage);
        }

        private void OpenFullImage(OpenFullImageMessage fullImage)
        {
            var imageViewer = _iocKernel.Get<FullImageBrowsingViewModel>();
            imageViewer.Init(fullImage.BooruImage, 
                fullImage._booruPosts,
                fullImage.BooruImageWrapperList,
                fullImage.QueryPage);

            CurrentView = imageViewer;
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
