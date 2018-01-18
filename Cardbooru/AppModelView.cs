using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Cardbooru.Helpers.Base;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

namespace Cardbooru
{
    public class AppModelView : INotifyPropertyChanged {

        private IUserControlViewModel _currentView;
        private IMvxMessenger _messenger;
        private IDisposable _token;
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
            _messenger = IdkInjection.MessengerHub;
            _token = _messenger.Subscribe<OpenFullImageMessage>(ShowFullImage);
            CurrentView = new BrowseImagesViewModel();
            ViewModels.Add(CurrentView);
        }

        private void ShowFullImage(OpenFullImageMessage fullImage) {
            var fullImageView = new FullImageBrowseViewModel(fullImage.BooruImage.FullImage.Source);
            CurrentView = fullImageView;
        }

        private void ChangeView(IUserControlViewModel viewModel) {
            if(!ViewModels.Contains(viewModel))
                ViewModels.Add(viewModel);

            CurrentView = ViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
