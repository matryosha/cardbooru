using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cardbooru;

namespace TestFromWpf
{
    
    public partial class MainWindow : Window
    {


        List<Button> listsButtons = new List<Button>();
        private BooruWorker _booruWorker;
        int index = 0;
        BooruImage booruImage;
        public MainWindow()
        {
            InitializeComponent();
            using (var fStream = File.OpenRead("default.jpg"))
            {
                MainImage.Source = BitmapFrame.Create(fStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            //MainImage.Source = BitmapFrame.Create();
        }

        private async void  ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            _booruWorker = new BooruWorker();

            Image image = new Image();
            

            image.Source = await _booruWorker.GetPreviewImage(booruImage = new BooruImage()
            {
                Hash = "d34e4cf0a437a5d65f8e82b7bcd02606",
                Id = "2",
                PreviewUrl = "/data/preview/d34e4cf0a437a5d65f8e82b7bcd02606.jpg"
            });

            booruImage.PreviewImage = new Image();
            booruImage.PreviewImage.Source = await _booruWorker.GetPreviewImage(booruImage);

            Binding myBinding = new Binding("PreviewImageSource");
            myBinding.Source = booruImage;
            MainImage.SetBinding(Image.SourceProperty, myBinding);

            await _booruWorker.FillBooruImages(1);
        }

        private void Action(object sender, RoutedEventArgs e) {
            booruImage.PreviewImage = _booruWorker.BooruImages[index].PreviewImage;
            index++;
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
