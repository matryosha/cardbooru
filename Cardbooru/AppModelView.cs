using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cardbooru
{
    public class AppModelView : INotifyPropertyChanged {

        private MainWindowViewModel _currentView;

        public MainWindowViewModel CurrentView {
            get => _currentView;
            set {
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }

        public AppModelView() {
            CurrentView = new MainWindowViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
