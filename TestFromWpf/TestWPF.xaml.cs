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
        private Model model;
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
            model = new Model();

            Image image = new Image();
            

            image.Source = await model.GetPreviewImage(booruImage = new BooruImage()
            {
                Hash = "d34e4cf0a437a5d65f8e82b7bcd02606",
                Id = "2",
                PreviewUrl = "/data/preview/d34e4cf0a437a5d65f8e82b7bcd02606.jpg"
            });

            booruImage.PreviewImage = new Image();
            booruImage.PreviewImage.Source = await model.GetPreviewImage(booruImage);

            Binding myBinding = new Binding("PreviewImageSource");
            myBinding.Source = booruImage;
            MainImage.SetBinding(Image.SourceProperty, myBinding);

            await model.GetImages(1);
        }

        private void Action(object sender, RoutedEventArgs e) {
            booruImage.PreviewImage = model.BooruImagesList[index].PreviewImage;
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
