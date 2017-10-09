using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Cardbooru;

namespace TestFromWpf
{
    
    public partial class MainWindow : Window
    {


        List<Button> listsButtons = new List<Button>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void  ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            Model model = new Model();

            Image image = new Image();

            //image.Source =  await model.GetPreviewImage(new BooruImage {
            //    Hash = "d34e4cf0a437a5d65f8e82b7bcd02606",
            //    Id = "2",
            //    PreviewUrl = "/data/preview/d34e4cf0a437a5d65f8e82b7bcd02606.jpg"
            //});

            MainImage.Source = image.Source;

            listsButtons.Add(new Button());
            listsButtons.Add(new Button());
            listsButtons.Add(new Button());
            listsButtons.Add(new Button());
            listsButtons.Add(new Button());

            
        }
    }
}
