using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cardbooru
{
    public class MainWidowViewModel : INotifyPropertyChanged {
        private int currentPage;

        private BooruWorker booruWorker; // TODO Make Interface

        public ObservableCollection<BooruImage> BooruImages { get; set; } = 
            new ObservableCollection<BooruImage>();
        

        public MainWidowViewModel() {
            currentPage = 1;
            booruWorker = new BooruWorker();
        }


        #region Commands

        private RelayCommand loadPreviewImages;
        public RelayCommand LoadCommand => loadPreviewImages ?? 
            (loadPreviewImages = new RelayCommand(async o => {
                await booruWorker.FillBooruImages(currentPage, BooruImages);
            }));

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
