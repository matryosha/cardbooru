using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cardbooru;
using Cardbooru.Helpers;
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

        private async void Action(object sender, RoutedEventArgs e) {

            var worker = new BooruWorker();
            var booru=  new BooruImageModel();
            booru.FullUrl =
                "/data/__ayase_eli_and_sonoda_umi_love_live_and_love_live_school_idol_project_drawn_by_skull573__051f3655cc45c8c57c050d45516ef6ed.png";
            booru.Hash = "test-full";
            await worker.LoadFullImage(booru);
            MainImage.Source = booru.FullImage.Source;

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
