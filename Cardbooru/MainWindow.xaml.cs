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
            
            var contex = DataContext as MainWindowViewModel;
            var list = sender as ListBox; 
            contex.OpenFullCommnad.Execute(list.SelectedItem);
        }
    }
}
