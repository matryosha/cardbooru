using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Cardbooru
{

    public partial class MainWindow : Window
    {

        private MainWidowViewModel model = new MainWidowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = model;
            
            
        }

        private  async void Action(object sender, RoutedEventArgs e) {
           //await model.FillBooruImages(1);
           
           Console.WriteLine(model.BooruImages.Count.ToString());
        }

        private void Action2(object sender, RoutedEventArgs e) {
            
        }
    }
}
