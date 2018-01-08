using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Cardbooru
{

    public partial class MainWindow : Window
    {

        private Model model = new Model();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = model;
            
            
        }

        private  async void Action(object sender, RoutedEventArgs e) {
           await model.GetImages(1);
           
           Console.WriteLine(model.BooruImagesList.Count.ToString());
        }

        private void Action2(object sender, RoutedEventArgs e) {
            
        }
    }
}
