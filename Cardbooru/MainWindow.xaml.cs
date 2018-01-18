using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cardbooru
{

    public partial class MainWindow : UserControl
    {

        public MainWindow()
        {
            InitializeComponent();      
        }

        private void Item_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            
            
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e) {
            var contex = DataContext as MainWindowViewModel;
            var list = sender as ListBoxItem;
            contex.OpenFullCommnad.Execute(list.Content);
        }
    }
}
