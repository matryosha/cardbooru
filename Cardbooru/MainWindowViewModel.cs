using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cardbooru
{
    public class MainWindowViewModel : INotifyPropertyChanged {
        private int _currentPage;
        private bool _isLoadling;

        private BooruWorker booruWorker; // TODO Make Interface

        public ObservableCollection<BooruImage> BooruImages { get; set; } = 
            new ObservableCollection<BooruImage>();
        

        public MainWindowViewModel() {
            _currentPage = 1;
            booruWorker = new BooruWorker();
        }


        #region Commands

        private RelayCommand loadPreviewImages;
        public RelayCommand LoadCommand => loadPreviewImages ?? 
            (loadPreviewImages = new RelayCommand(async o => {
                if(_isLoadling) return;
                _isLoadling = true;
                await booruWorker.FillBooruImages(_currentPage, BooruImages);
                _currentPage++;
                _isLoadling = false;
            }));

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
