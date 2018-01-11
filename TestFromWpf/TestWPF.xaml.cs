using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cardbooru;
using WpfAnimatedGif;

namespace TestFromWpf
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void  ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            
        }

        private void Action(object sender, RoutedEventArgs e) {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/ajax-loader (2).gif");
            image.EndInit();
            //MainImage.Width = 50;
            //MainImage.Height = 50;
            ImageBehavior.SetAnimatedSource(MainImage, image);
            MainImage.Stretch = Stretch.None;

        }

        private void SaveImageToJPEG(Image ImageToSave, string Location)
        {
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)ImageToSave.Source.Width,
                (int)ImageToSave.Source.Height,
                100, 100, PixelFormats.Default);
            renderTargetBitmap.Render(ImageToSave);
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (FileStream fileStream = new FileStream(Location, FileMode.Create))
            {
                jpegBitmapEncoder.Save(fileStream);
            }
        }


    }
}
